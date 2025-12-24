using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkedManagerPrefabs", menuName = "Scriptable Objects/NetworkedManagerPrefabs")]
public class NetworkedManagerPrefabs : ScriptableObject
{
    [SerializeField] private NetworkObject[] m_ManagerPrefabs;

    public NetworkObject[] ManagerPrefabs => m_ManagerPrefabs;
}
