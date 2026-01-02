using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class Workstation : NetworkBehaviour
    {
        [SerializeField] private ItemSlot m_ItemSlot;

        public ItemSlot ItemSlot => m_ItemSlot;

        public void TransferItem(NetworkObject player)
        {
            TransferItemRpc(player);
        }

        protected virtual void OnItemRecieved(Item item) { }
        protected virtual void OnItemRemoved(Item item) { }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void TransferItemRpc(NetworkObjectReference playerNetObjRef)
        {
            if (!playerNetObjRef.TryGet(out NetworkObject playerNetworkObj)) return;

            // Provided NetworkObject must be a player, if not we exit early
            if (!playerNetworkObj.TryGetComponent(out Player player)) return;

            // If successful in transferring from player to this, return
            if (player.ItemSlot.TryTransferHeldItem(ItemSlot))
            {
                OnItemRecieved(ItemSlot.CurrentHeldItem);
                return;
            }

            // If successful in transferring from this to player, return
            if (ItemSlot.TryTransferHeldItem(player.ItemSlot))
            {
                OnItemRemoved(player.ItemSlot.CurrentHeldItem);
                return;
            }

            // If neither transfer works, do nothing
        }
    }
}