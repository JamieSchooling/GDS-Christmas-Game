using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class ClientJoin : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            Debug.Log("Spawned ClientJoin");

            ServerPlayerStateManager.Instance.SubmitClientInfoRpc();
        }
    }
}