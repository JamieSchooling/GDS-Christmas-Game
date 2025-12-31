using System.Linq;
using UnityEngine;

namespace GDS
{
    [CreateAssetMenu(fileName = "LevelItems", menuName = "Scriptable Objects/LevelItems")]
    public class LevelItems : ScriptableObject
    {
        [SerializeField] private ItemData[] m_LevelItems;

        public ItemData[] AllLevelItems => m_LevelItems;
        public Component[] LevelComponents => m_LevelItems.Where(item => item is Component).ToArray() as Component[];
        public Toy[] LevelToys => m_LevelItems.Where(item => item is Toy).ToArray() as Toy[];
        
        public int IndexOf(ItemData itemData)
        {
            return m_LevelItems.ToList().IndexOf(itemData);
        }
    }
}