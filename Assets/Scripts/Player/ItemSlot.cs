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
        }

        public bool TryTransferHeldItem(ItemSlot newSlot)
        {
            if (CurrentHeldItem == null || newSlot == null) return false;

            if (newSlot.ContainsItem) return false;

            newSlot.SetCurrentHeldItem(CurrentHeldItem);
            CurrentHeldItem = null;

            return true;
        }
    }
}
