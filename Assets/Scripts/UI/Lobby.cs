using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class Lobby : NetworkBehaviour
    {
        [SerializeField] private PlayerLobbyEntry m_LobbyEntryPrefab;
        [SerializeField] private RectTransform m_PlayerList;
        [SerializeField] private TextMeshProUGUI m_JoinCodeDisplay;

        private Dictionary<string, PlayerLobbyEntry> m_DisplayNameToLobbyEntry = new();

        private enum EntryUpdateType
        {
            Add,
            Remove
        }

        public override void OnNetworkSpawn()
        {
            string joinCode = RelayConnectionManager.JoinCode;
            if (joinCode != null)
                m_JoinCodeDisplay.text = joinCode;

            if (!IsServer) return;

            ServerConnectionRegistry.Instance.OnPlayerConnected += NotifyClientsLobbyUpdate;
            ServerConnectionRegistry.Instance.OnPlayerDisconnected += NotifyClientsLobbyUpdate;

            foreach (PlayerState state in ServerConnectionRegistry.Instance.AllPlayerStates)
            {
                UpdateLobbyEntryRpc
                (
                    state.DisplayName,
                    state.IsConnected ?
                    EntryUpdateType.Add :
                    EntryUpdateType.Remove
                );
            }

        }

        private void NotifyClientsLobbyUpdate(PlayerState state)
        {
            Debug.Log("PlayerStateUpdated event recieved here, might not be server");
            if (!IsServer) return;
            Debug.Log("PlayerStateUpdated event recieved here, am server");

            foreach (PlayerState s in ServerConnectionRegistry.Instance.AllPlayerStates)
            {
                Debug.Log($"IsConnected: {s.IsConnected}");

                UpdateLobbyEntryRpc
                (
                    s.DisplayName,
                    s.IsConnected ?
                    EntryUpdateType.Add :
                    EntryUpdateType.Remove
                );
            }
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void UpdateLobbyEntryRpc(string displayName, EntryUpdateType updateType)
        {
            Debug.Log(updateType);

            if (updateType == EntryUpdateType.Add)
            {
                if (m_DisplayNameToLobbyEntry.ContainsKey(displayName)) return;

                PlayerLobbyEntry lobbyEntry = Instantiate(
                    m_LobbyEntryPrefab
                );

                lobbyEntry.transform.SetParent(m_PlayerList, false);

                lobbyEntry.Init(displayName);
                m_DisplayNameToLobbyEntry.Add(displayName, lobbyEntry.GetComponent<PlayerLobbyEntry>());
            }
            if (updateType == EntryUpdateType.Remove)
            {
                if (!m_DisplayNameToLobbyEntry.TryGetValue(displayName, out PlayerLobbyEntry entry))
                    return;

                Destroy(entry.gameObject);
                m_DisplayNameToLobbyEntry.Remove(displayName);
            }
        }
    }
}