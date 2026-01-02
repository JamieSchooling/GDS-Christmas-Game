using GDS;
using Unity.Netcode;
using UnityEngine;

public class ComponentStation : NetworkBehaviour
{
    [SerializeField] private Item m_ItemPrefab;
    [SerializeField] private LevelItems m_LevelItems;
    [SerializeField] private ItemData m_ComponentData;

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
        Item item = component.GetComponent<Item>();
        item.SetDataRpc(m_LevelItems.IndexOf(m_ComponentData), m_ComponentData.ItemType);
        player.ItemSlot.SetCurrentHeldItem(item);
    }
}
