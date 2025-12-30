using UnityEngine;

[CreateAssetMenu(fileName = "Component", menuName = "Scriptable Objects/Component")]
public class Component : ScriptableObject
{
    [SerializeField] string m_Name;

    public string Name => m_Name;
}
