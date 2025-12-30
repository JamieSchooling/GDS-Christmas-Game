using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class Item : NetworkBehaviour
    {
        public void SetOwnerSlot(ItemSlot ownerSlot)
        {
            if (ownerSlot == null)
                return;

            NetworkObjectReference slotRef = ownerSlot.NetworkObject;

            SetOwnerSlotRpc(slotRef);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void SetOwnerSlotRpc(NetworkObjectReference ownerSlot)
        {
            if (!ownerSlot.TryGet(out NetworkObject ownerSlotNetObj)) return;
            if (!ownerSlotNetObj.TryGetComponent(out ItemSlot itemSlot))
            {
                Debug.LogError("Item owner couldn't be set: owner object does not contain an ItemSlot");
                return;
            }

            transform.position = itemSlot.Position;

            if (!NetworkObject.TrySetParent(ownerSlotNetObj))
            {
                Debug.LogError("Couldn't parent item to item slot");
                return;
            }

            NetworkObject.ChangeOwnership(ownerSlotNetObj.OwnerClientId);
        }
    }
}
