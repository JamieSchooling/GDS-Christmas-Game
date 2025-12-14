using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class PlayerState
    {
        public string PlayerID { get; }
        public ulong ClientID { get; set; }

        public PlayerState(string playerID)
        {
            PlayerID = playerID;
        }
    }

    public class ServerPlayerStateManager : NetworkBehaviour
    {
        [SerializeField] private ClientJoin clientJoinPrefab;

        public static ServerPlayerStateManager Instance { get; private set; } = null;

        private Dictionary<string, PlayerState> m_PlayerStates = new Dictionary<string, PlayerState>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            NetworkManager.Singleton.OnConnectionEvent += SpawnClientJoin;
        }

        private void SpawnClientJoin(NetworkManager networkManager, ConnectionEventData connectionEventData)
        {
            if (!IsServer) return;

            if (connectionEventData.EventType != ConnectionEvent.ClientConnected) return;

            NetworkObject.InstantiateAndSpawn
            (
                clientJoinPrefab.gameObject, 
                networkManager, 
                connectionEventData.ClientId, 
                destroyWithScene: true
            );
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    NetworkObject.InstantiateAndSpawn
                    (
                        clientJoinPrefab.gameObject,
                        NetworkManager.Singleton,
                        client,
                        destroyWithScene: true
                    );
                }
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void SubmitClientInfoRpc(RpcParams rpcParams = default)
        {
            if (!IsServer) return;

            ulong clientID = rpcParams.Receive.SenderClientId;

            if (!ServerConnectionRegistry.Instance.TryGetPlayerId(clientID, out string playerID))
                return; // Throw?

            if (!m_PlayerStates.TryGetValue(playerID, out PlayerState playerState))
            {
                playerState = new PlayerState(playerID);
                m_PlayerStates.Add(playerID, playerState);
            }

            playerState.ClientID = clientID;
        }
    }
}