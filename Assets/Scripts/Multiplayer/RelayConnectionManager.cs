using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;

namespace GDS
{
    public class RelayConnectionManager : MonoBehaviour
    {
        [SerializeField] private int m_MaxPlayers = 4;

        public UnityEvent OnConnectionEstablished;
        public UnityEvent OnConnectionFailed;

        public static string JoinCode = null;

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

                if (!NetworkManager.Singleton.StartHost())
                {
                    OnConnectionFailed?.Invoke();
                    return null;
                }

                OnConnectionEstablished?.Invoke();

                Debug.Log(joinCode);
                JoinCode = joinCode;
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                OnConnectionFailed?.Invoke();
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

                if (!NetworkManager.Singleton.StartClient())
                {
                    OnConnectionFailed?.Invoke();
                    return;
                }

                OnConnectionEstablished?.Invoke();
            }
            catch (RelayServiceException e)
            {
                OnConnectionFailed?.Invoke();
                Debug.LogError(e);
            }
        }


    }

}
