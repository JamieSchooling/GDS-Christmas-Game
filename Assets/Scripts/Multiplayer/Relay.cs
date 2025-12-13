using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace GDS
{
    public class Relay : MonoBehaviour
    {
        [SerializeField] private int m_MaxPlayers = 4;

        public async Task<string> CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(m_MaxPlayers-1);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                );

                NetworkManager.Singleton.StartHost();

                Debug.Log(joinCode);
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        public async void JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData,
                    allocation.HostConnectionData
                );

                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
            }
        }
    }
}
