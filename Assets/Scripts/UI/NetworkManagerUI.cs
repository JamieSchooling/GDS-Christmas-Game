using GDS;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button m_HostButton;
    [SerializeField] private Button m_JoinButton;
    [SerializeField] private TMP_InputField m_JoinCodeField;
    [SerializeField] private TextMeshProUGUI m_JoinCodeDisplay;
    [SerializeField] private Relay m_Relay;

    private void Awake()
    {
        m_HostButton.onClick.AddListener(async () =>
        {
            m_JoinCodeDisplay.text = "Creating...";
            m_JoinCodeDisplay.text = await m_Relay.CreateRelay();
        });

        m_JoinButton.onClick.AddListener(() =>
        {
            m_Relay.JoinRelay(m_JoinCodeField.text);
        });
    }
}
