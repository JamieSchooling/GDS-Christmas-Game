using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class ClientJoin : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            PlayerProfileData profileData = new();
            if (!PlayerProfileSerialiser.TryLoadProfileData(out profileData))
                Debug.LogError("Couldn't load profile data to submit to server.");

            ServerPlayerStateManager.Instance.SubmitClientInfoRpc(profileData.DisplayName);
        }
    }
}