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
        Debug.Log("Transfering");

        if (!playerNetObjRef.TryGet(out NetworkObject playerNetworkObj)) return;

        // Provided NetworkObject must be a player, if not we exit early
        if (!playerNetworkObj.TryGetComponent(out Player player))
        {
            Debug.Log("Net obj not player");
            return;
        }


        // If successful in transferring from player to this, return
        if (player.ItemSlot.TryTransferHeldItem(ItemSlot)) return;

        Debug.Log("Couldn't transfer from player to station");

        // If successful in transferring from this to player, return
        if (ItemSlot.TryTransferHeldItem(player.ItemSlot)) return;

        Debug.Log("Couldn't transfer from station to player");
        // If neither transfer works, do nothing

        Debug.Log("Couldn't transfer at all");
    }
}
