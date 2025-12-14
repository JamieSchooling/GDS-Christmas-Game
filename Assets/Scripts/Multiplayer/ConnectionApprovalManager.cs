using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class ConnectionApprovalManager : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApproval;
        }

        void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.ConnectionApprovalCallback -= ConnectionApproval;
        }

        private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            string playerID = Encoding.UTF8.GetString(request.Payload);
            ulong clientID = request.ClientNetworkId;

            ServerConnectionRegistry.Instance.Bind(clientID, playerID);

            // Could do additional checks here like is session full, or a whitelist system

            response.Approved = true;
            response.CreatePlayerObject = false;
        }
    }
}
