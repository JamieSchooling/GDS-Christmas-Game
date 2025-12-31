using UnityEngine;

namespace GDS
{
    public class ItemData : ScriptableObject
    {
        [SerializeField] private Sprite m_Sprite;
        [SerializeField] private string m_ItemName;

        public string Name => m_ItemName;
        public Sprite Sprite => m_Sprite;
    }
}