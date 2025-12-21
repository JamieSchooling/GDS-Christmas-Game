using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class ServerConnectionRegistry : NetworkBehaviour
    {
        [SerializeField] private ClientJoin m_ClientJoinPrefab;

        public static ServerConnectionRegistry Instance { get; private set; } = null;
        public PlayerState[] AllPlayerStates => m_PlayerStates.Values.ToArray();

        public Action<PlayerState> OnPlayerConnected;
        public Action<PlayerState> OnPlayerDisconnected;

        private Dictionary<string, PlayerState> m_PlayerStates = new();
        private Dictionary<ulong, string> m_ClientIdToPlayerId = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(this);
        }

        public void RegisterCallbacks()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (clientID) => SpawnClientJoin(clientID);
            NetworkManager.Singleton.OnClientDisconnectCallback += (clientID) => Unbind(clientID);
            NetworkManager.Singleton.OnServerStopped += (bool b) => m_ClientIdToPlayerId.Clear();
            NetworkManager.Singleton.OnClientStopped += (bool b) => m_ClientIdToPlayerId.Clear();
        }

        public void Bind(ulong clientId, string playerId)
        {
            m_ClientIdToPlayerId[clientId] = playerId;
        }

        public void Unbind(ulong clientId)
        {
            Debug.Log($"Client: {clientId} disconnected");

            if (TryGetPlayerId(clientId, out var playerId) && 
                TryGetPlayerState(playerId, out PlayerState playerState))
            {
                playerState.IsConnected = false;
                OnPlayerDisconnected?.Invoke(playerState);
                m_ClientIdToPlayerId.Remove(clientId);
            }
        }

        public bool TryGetPlayerId(ulong clientId, out string playerId)
        {
            return m_ClientIdToPlayerId.TryGetValue(clientId, out playerId);
        }

        public bool TryGetPlayerState(string playerID, out PlayerState playerState)
        {
            return m_PlayerStates.TryGetValue(playerID, out playerState);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void SubmitClientInfoRpc(string displayName, RpcParams rpcParams = default)
        {
            if (!IsServer) return;

            ulong clientID = rpcParams.Receive.SenderClientId;

            if (!TryGetPlayerId(clientID, out string playerID))
                return; // Throw?

            if (!TryGetPlayerState(playerID, out PlayerState playerState))
            {
                playerState = new PlayerState(playerID);
                m_PlayerStates.Add(playerID, playerState);
            }

            Debug.Log(displayName);

            playerState.ClientID = clientID;
            playerState.DisplayName = displayName;
            playerState.IsConnected = true;

            OnPlayerConnected?.Invoke(playerState);
        }

        private void SpawnClientJoin(ulong clientID)
        {
            if (!IsServer) return;

            NetworkObject.InstantiateAndSpawn
            (
                m_ClientJoinPrefab.gameObject,
                NetworkManager.Singleton,
                clientID,
                destroyWithScene: true
            );
        }
    }
}
