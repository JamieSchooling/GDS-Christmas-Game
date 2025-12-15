using GDS;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Lobby : NetworkBehaviour
{
    [SerializeField] private PlayerLobbyItem m_LobbyItemPrefab;
    [SerializeField] private RectTransform m_PlayerList;
    [SerializeField] private TextMeshProUGUI m_JoinCodeDisplay;

    private Dictionary<string, PlayerLobbyItem> m_DisplayNameToLobbyItem = new();

    public override void OnNetworkSpawn()
    {
        string joinCode = RelayConnectionManager.JoinCode;
        if (joinCode != null)
            m_JoinCodeDisplay.text = joinCode;

        if (!IsServer) return;

        ServerPlayerStateManager.Instance.OnPlayerStateUpdated += NotifyClientsLobbyUpdate;

        foreach (PlayerState state in ServerPlayerStateManager.Instance.AllPlayerStates)
        {
            UpdateLobbyItemRpc
            (
                state.DisplayName,
                state.IsConnected ? 
                ConnectionEvent.ClientConnected : 
                ConnectionEvent.ClientDisconnected
            );
        }

    }

    private void NotifyClientsLobbyUpdate(PlayerState state, ConnectionEvent connectionType)
    {
        Debug.Log("PlayerStateUpdated event recieved here, might not be server");
        if (!IsServer) return;
        Debug.Log("PlayerStateUpdated event recieved here, am server");

        foreach (PlayerState s in ServerPlayerStateManager.Instance.AllPlayerStates)
        {
            UpdateLobbyItemRpc
            (
                s.DisplayName,
                s.IsConnected ?
                ConnectionEvent.ClientConnected :
                ConnectionEvent.ClientDisconnected
            );
        }
    }

    //public override void OnNetworkSpawn()
    //{
    //    foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
    //    {
    //        ConnectionEventData data = new();
    //        data.ClientId = client;
    //        data.EventType = ConnectionEvent.ClientConnected;
    //        //UpdateLobbyItems(NetworkManager.Singleton, data);
    //    }
    //}

    [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
    private void UpdateLobbyItemRpc(string displayName, ConnectionEvent connectionType)
    {
        Debug.Log(connectionType);

        if (connectionType == ConnectionEvent.ClientConnected)
        {
            if (m_DisplayNameToLobbyItem.ContainsKey(displayName)) return;

            PlayerLobbyItem lobbyItem = Instantiate(
                m_LobbyItemPrefab
            );

            lobbyItem.transform.SetParent(m_PlayerList, false);

            lobbyItem.Init(displayName);
            m_DisplayNameToLobbyItem.Add(displayName, lobbyItem.GetComponent<PlayerLobbyItem>());
        }
        else if (connectionType == ConnectionEvent.ClientDisconnected)
        {
            if (!m_DisplayNameToLobbyItem.ContainsKey(displayName)) return;

            Destroy(m_DisplayNameToLobbyItem[displayName]);
            m_DisplayNameToLobbyItem.Remove(displayName);
        }
    }
}
