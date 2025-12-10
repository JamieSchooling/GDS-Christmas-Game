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
        if (!playerRef.TryGet(out NetworkObject player))
            return;

        NetworkObject component 
            = Instantiate(m_ComponentPrefab, player.transform.position + new Vector3(2, 0, 0), Quaternion.identity);
        component.Spawn();
        component.TrySetParent(player);
    }
}
