using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class Item : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private LevelItems m_LevelItems;

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        public void SetDataRpc(int itemDataIndex, ItemType type)
        {
            ItemData data = type switch
            {
                ItemType.BaseComponent => m_LevelItems.BaseComponents[itemDataIndex],
                ItemType.CraftableComponent => m_LevelItems.CraftableComponents[itemDataIndex],
                ItemType.Toy => m_LevelItems.Toys[itemDataIndex],
                _ => throw new System.NotImplementedException(),
            };

            m_SpriteRenderer.sprite = data.Sprite;
        }

        public void SetOwnerSlot(ItemSlot ownerSlot)
        {
            if (ownerSlot == null)
                return;

            SetOwnerSlotRpc(ownerSlot.NetworkObject);
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
