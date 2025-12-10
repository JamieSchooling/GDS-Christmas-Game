using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [SerializeField] string ItName;
    [SerializeField] Component[] m_Components;

    public Component[] Components => m_Components;
    public string Name => ItName;
}
