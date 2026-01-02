using UnityEngine;

namespace GDS
{
    public class ConversionStation : Workstation
    {
        protected override void OnItemRecieved(Item item)
        {
            if (item.Data.ItemType is ItemType.ConvertibleComponent)
            {
                item.SetDataRpc(ItemSlot.LevelItems.IndexOf(item.Data.ConversionResult), item.Data.ConversionResult.ItemType);
            }
        }
    }
}
