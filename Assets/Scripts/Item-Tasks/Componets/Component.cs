using UnityEngine;

[CreateAssetMenu(fileName = "Component", menuName = "Scriptable Objects/Component")]
public class Component : ScriptableObject
{
    [SerializeField] string ComName;

    public string Name => ComName;
}
