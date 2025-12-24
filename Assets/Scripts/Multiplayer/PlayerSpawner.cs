using GDS;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private Player m_PlayerPrefab;

    public override void OnNetworkSpawn()
    {
        foreach (PlayerState p in ServerConnectionRegistry.Instance.AllPlayerStates)
        {
           NetworkObject.InstantiateAndSpawn
           (
               m_PlayerPrefab.gameObject,
               NetworkManager.Singleton,
               p.ClientID,
               destroyWithScene: true
           );
        }
    }
}
