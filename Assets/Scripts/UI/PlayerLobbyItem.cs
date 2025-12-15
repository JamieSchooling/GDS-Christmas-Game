using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerLobbyItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_DisplayName;

    public void Init(string displayName)
    {
        m_DisplayName.text = displayName;
    }
}
