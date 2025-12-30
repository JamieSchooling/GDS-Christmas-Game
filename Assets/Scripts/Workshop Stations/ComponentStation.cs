using GDS;
using Unity.Netcode;
using UnityEngine;

public class ComponentStation : NetworkBehaviour
{
    [SerializeField] private NetworkObject m_ComponentPrefab;

    public void AssignComponent(NetworkObject player)
    {
        if (player == null)
            return;

        NetworkObjectReference playerRef = player;

        AssignRpc(playerRef);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AssignRpc(NetworkObjectReference playerRef)
    {
        if (!playerRef.TryGet(out NetworkObject playerNetObj)) return;

        if (!playerNetObj.TryGetComponent(out Player player)) return;

        NetworkObject component = NetworkObject.InstantiateAndSpawn
        (
            m_ComponentPrefab.gameObject, 
            NetworkManager.Singleton, 
            ownerClientId: playerNetObj.OwnerClientId,
            position: player.HoldPoint.position, 
            rotation: Quaternion.identity
        );
        component.TrySetParent(playerNetObj);
    }
}
