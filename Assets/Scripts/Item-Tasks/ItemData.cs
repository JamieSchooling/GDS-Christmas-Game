using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GDS
{
    public enum ItemType
    {
        BaseComponent,
        CraftableComponent,
        Toy
    }

    [CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Objects/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private Sprite m_Sprite;
        [SerializeField] private string m_ItemName;
        [SerializeField] private ItemType m_ItemType;

        // Hidden in inspector to allow for custom editor to draw
        [SerializeField, HideInInspector] private ItemData[] m_Components;

        public string Name => m_ItemName;
        public Sprite Sprite => m_Sprite;
        public ItemType ItemType => m_ItemType;
        public ItemData[] Components => m_ItemType != ItemType.BaseComponent ? m_Components : null;

        private void OnValidate()
        {
            if (m_ItemType != ItemType.BaseComponent)
            {
                bool containedInvalid = false;
                for (int i = 0; i < m_Components.Length; ++i)
                {
                    if (m_Components[i] == null) continue;
                    if (m_ItemType is ItemType.CraftableComponent && m_Components[i].ItemType == ItemType.Toy)
                    {
                        m_Components[i] = null;
                        containedInvalid = true;
                    }
                    else if (m_ItemType is ItemType.Toy && m_Components[i].ItemType is ItemType.BaseComponent or ItemType.Toy)
                    {
                        m_Components[i] = null;
                        containedInvalid = true;
                    }
                }

#if UNITY_EDITOR
                if (containedInvalid)
                    EditorUtility.DisplayDialog(
                        "Invalid Item Type",
                        "Component must be of a valid type. (Toys cannot accept base components)",
                        "Ok"
                    );
#endif
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemData))]
    public class ItemDataEditor : Editor
    {
        private SerializedProperty m_ItemTypeProp;
        private SerializedProperty m_ComponentsProp;

        private void OnEnable()
        {
            m_ItemTypeProp = serializedObject.FindProperty("m_ItemType");
            m_ComponentsProp = serializedObject.FindProperty("m_Components");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_ItemTypeProp.enumValueIndex == (int)ItemType.CraftableComponent ||
                m_ItemTypeProp.enumValueIndex == (int)ItemType.Toy)
            {
                EditorGUILayout.PropertyField(m_ComponentsProp, true);
                if (m_ComponentsProp.arraySize <= 2)
                    serializedObject.ApplyModifiedProperties();
            }

        }
    }
#endif
}