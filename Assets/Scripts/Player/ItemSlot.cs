using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public class ItemSlot : NetworkBehaviour
    {
        [SerializeField] private Transform m_SlotPoint;
        [SerializeField] private LevelItems m_LevelItems;

        public Item CurrentHeldItem { get; private set; } = null;
        public bool ContainsItem => CurrentHeldItem != null;
        public Vector2 Position => m_SlotPoint.position;
        public LevelItems LevelItems => m_LevelItems;

        public void SetCurrentHeldItem(Item item)
        {
            CurrentHeldItem = item;
            CurrentHeldItem.SetOwnerSlot(this);
        }

        public bool TryTransferHeldItem(ItemSlot newSlot)
        {
            if (CurrentHeldItem == null) return false;

            if (newSlot == null) return false;

            if (newSlot.ContainsItem)
            {
                if (m_LevelItems == null)
                {
                    Debug.LogError("LevelItems null");
                    return false;
                }
                if (m_LevelItems.TryCraft(CurrentHeldItem.Data, newSlot.CurrentHeldItem.Data, out ItemData result))
                {
                    newSlot.CurrentHeldItem.SetDataRpc(m_LevelItems.IndexOf(result), result.ItemType);
                    Destroy(CurrentHeldItem.gameObject);
                    CurrentHeldItem = null;
                    return true;
                }
                return false;
            }

            newSlot.SetCurrentHeldItem(CurrentHeldItem);
            CurrentHeldItem = null;

            return true;
        }
    }
}
