using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "Toy", menuName = "Scriptable Objects/Toy")]
public class Toy : ScriptableObject
{
    [SerializeField] string m_Name;
    [SerializeField] Component[] m_Components;

    public Component[] Components => m_Components;
    public string Name => m_Name;
}
