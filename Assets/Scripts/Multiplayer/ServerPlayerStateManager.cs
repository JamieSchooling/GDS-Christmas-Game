using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class PlayerState
    {
        public string PlayerID { get; }
        public ulong ClientID { get; set; }
        public string DisplayName { get; set; }
        public bool IsConnected { get; set; }

        public PlayerState(string playerID)
        {
            PlayerID = playerID;
        }
    }

    public class ServerPlayerStateManager : NetworkBehaviour
    {
        [SerializeField] private ClientJoin clientJoinPrefab;

        public static ServerPlayerStateManager Instance { get; private set; } = null;
        public PlayerState[] AllPlayerStates => m_PlayerStates.Values.ToArray();

        public Action<PlayerState, ConnectionEvent> OnPlayerStateUpdated;

        private Dictionary<string, PlayerState> m_PlayerStates = new();

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

            if (connectionEventData.EventType == ConnectionEvent.ClientConnected)
            {
                NetworkObject.InstantiateAndSpawn
                (
                    clientJoinPrefab.gameObject,
                    networkManager,
                    connectionEventData.ClientId,
                    destroyWithScene: true
                );
            }
            else if (connectionEventData.EventType == ConnectionEvent.ClientDisconnected)
            {
                if (!ServerConnectionRegistry.Instance.TryGetPlayerId(connectionEventData.ClientId, out string playerID))
                    return;

                if (m_PlayerStates.TryGetValue(playerID, out PlayerState playerState))
                {
                    playerState.IsConnected = false;
                    OnPlayerStateUpdated?.Invoke(playerState, ConnectionEvent.ClientDisconnected);
                }
            }
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
        public void SubmitClientInfoRpc(string displayName, RpcParams rpcParams = default)
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
            playerState.DisplayName = displayName;
            playerState.IsConnected = true;

            OnPlayerStateUpdated?.Invoke(playerState, ConnectionEvent.ClientConnected);
            Debug.Log("PlayerStateUpdated event should have fired here");
        }

        public bool TryGetPlayerState(string playerID, out PlayerState playerState)
        {
            return m_PlayerStates.TryGetValue(playerID, out playerState);
        }
    }
}