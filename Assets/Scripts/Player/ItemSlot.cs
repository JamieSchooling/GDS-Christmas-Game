using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class ItemSlot : NetworkBehaviour
    {
        [SerializeField] private Transform m_SlotPoint;

        public Item CurrentHeldItem { get; private set; } = null;
        public bool ContainsItem => CurrentHeldItem != null;
        public Vector2 Position => m_SlotPoint.position;

        public void SetCurrentHeldItem(Item item)
        {
            CurrentHeldItem = item;
            CurrentHeldItem.SetOwnerSlot(this);
        }

        public bool TryTransferHeldItem(ItemSlot newSlot)
        {
            if (CurrentHeldItem == null)
            {
                Debug.LogError("Couldn't transfer item: Item is null");
                return false;
            }

            if (newSlot == null)
            {
                Debug.LogError("Couldn't transfer item: New ItemSlot is null");
                return false;
            }

            if (newSlot.ContainsItem)
            {
                Debug.LogError("Couldn't transfer item: New ItemSlot already has item");
                return false;
            }

            newSlot.SetCurrentHeldItem(CurrentHeldItem);
            CurrentHeldItem = null;

            return true;
        }
    }
}
