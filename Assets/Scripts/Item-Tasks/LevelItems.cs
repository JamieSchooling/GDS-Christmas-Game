using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GDS
{
    [CreateAssetMenu(fileName = "LevelItems", menuName = "Scriptable Objects/LevelItems")]
    public class LevelItems : ScriptableObject
    {
        [SerializeField] private ItemData[] m_BaseComponents;
        [SerializeField] private ItemData[] m_CraftableComponents;
        [SerializeField] private ItemData[] m_Toys;

        public ItemData[] BaseComponents => m_BaseComponents;
        public ItemData[] CraftableComponents => m_CraftableComponents;
        public ItemData[] Toys => m_Toys;
        
        public int IndexOf(ItemData itemData)
        {
            switch (itemData.ItemType)
            {
                case ItemType.BaseComponent:
                    return m_BaseComponents.ToList().IndexOf(itemData);
                case ItemType.CraftableComponent:
                    return m_CraftableComponents.ToList().IndexOf(itemData);
                case ItemType.Toy:
                    return m_Toys.ToList().IndexOf(itemData);
                default:
                    return -1;
            }
        }

        private void OnValidate()
        {
            bool containedInvalid = false;
            for (int i = 0; i < m_BaseComponents.Length; ++i)
            {
                if (m_BaseComponents[i] == null) continue;
                if (m_BaseComponents[i].ItemType != ItemType.BaseComponent)
                {
                    m_BaseComponents[i] = null;
                    containedInvalid = true;
                }
            }
            
            for (int i = 0; i < m_CraftableComponents.Length; ++i)
            {
                if (m_CraftableComponents[i] == null) continue;
                if (m_CraftableComponents[i].ItemType != ItemType.CraftableComponent)
                {
                    m_CraftableComponents[i] = null;
                    containedInvalid = true;
                }
            }
            
            for (int i = 0; i < m_Toys.Length; ++i)
            {
                if (m_Toys[i] == null) continue;
                if (m_Toys[i].ItemType != ItemType.Toy)
                {
                    m_Toys[i] = null;
                    containedInvalid = true;
                }
            }

#if UNITY_EDITOR
            if (containedInvalid)
                EditorUtility.DisplayDialog(
                    "Invalid Item Type",
                    "Item type must be of a valid type for specified list.",
                    "Ok"
                );
#endif
        }
    }
}