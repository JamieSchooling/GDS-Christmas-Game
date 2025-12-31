using GDS;
using UnityEngine;

[CreateAssetMenu(fileName = "Toy", menuName = "Scriptable Objects/Toy")]
public class Toy : ItemData
{
    [SerializeField] Component[] m_Components;

    public Component[] Components => m_Components;
}
