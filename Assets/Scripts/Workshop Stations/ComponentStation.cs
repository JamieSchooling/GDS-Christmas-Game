using GDS;
using Unity.Netcode;
using UnityEngine;

public class ComponentStation : NetworkBehaviour
{
    [SerializeField] private Item m_ItemPrefab;

    public void AssignComponent(NetworkObject player)
    {
        if (player == null)
            return;

        AssignRpc(player);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AssignRpc(NetworkObjectReference playerRef)
    {
        if (!playerRef.TryGet(out NetworkObject playerNetObj)) return;

        if (!playerNetObj.TryGetComponent(out Player player)) return;

        if (player.ItemSlot.ContainsItem) return;

        NetworkObject component = NetworkObject.InstantiateAndSpawn
        (
            m_ItemPrefab.gameObject,
            NetworkManager.Singleton
        );
        player.ItemSlot.SetCurrentHeldItem(component.GetComponent<Item>()); // Needs to run on client
    }
}
