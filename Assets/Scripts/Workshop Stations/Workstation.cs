using GDS;
using Unity.Netcode;
using UnityEngine;

public class Workstation : NetworkBehaviour
{
    [SerializeField] private InteractableNetworked m_Interactable;
    [SerializeField] private ItemSlot m_ItemSlot;

    public ItemSlot ItemSlot => m_ItemSlot;

    private void Awake()
    {
        m_Interactable.OnInteract.AddListener(TransferItem);
    }

    private void TransferItem(NetworkObject player)
    {
        TransferItemRpc(player);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void TransferItemRpc(NetworkObjectReference playerNetObjRef)
    {
        if (!playerNetObjRef.TryGet(out NetworkObject playerNetworkObj)) return;

        // Provided NetworkObject must be a player, if not we exit early
        if (!playerNetworkObj.TryGetComponent(out Player player)) return;

        // If successful in transferring from player to this, return
        if (player.ItemSlot.TryTransferHeldItem(ItemSlot)) return;

        // If successful in transferring from this to player, return
        if (ItemSlot.TryTransferHeldItem(player.ItemSlot)) return;

        // If neither transfer works, do nothing
    }
}
