using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class ServerConnectionRegistry : MonoBehaviour
    {
        public static ServerConnectionRegistry Instance { get; private set; } = null;

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

        private void Start()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += (clientID) => Unbind(clientID);
            NetworkManager.Singleton.OnServerStopped += (bool b) => clientIdToPlayerId.Clear();
            NetworkManager.Singleton.OnClientStopped += (bool b) => clientIdToPlayerId.Clear();
        }

        private Dictionary<ulong, string> clientIdToPlayerId = new();

        public void Bind(ulong clientId, string playerId)
        {
            clientIdToPlayerId[clientId] = playerId;
        }

        public bool TryGetPlayerId(ulong clientId, out string playerId)
        {
            return clientIdToPlayerId.TryGetValue(clientId, out playerId);
        }

        public void Unbind(ulong clientId)
        {
            clientIdToPlayerId.Remove(clientId);
        }
    }
}
