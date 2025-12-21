using TMPro;
using UnityEngine;

public class PlayerLobbyEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_DisplayName;

    public void Init(string displayName)
    {
        m_DisplayName.text = displayName;
    }
}
