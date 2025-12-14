using GDS;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button m_CreateGameButton;
    [SerializeField] private Button m_JoinGameButton;
    [SerializeField] private Button m_QuitButton;
    [SerializeField] private TMP_InputField m_JoinCodeField;
    [SerializeField] private TMP_InputField m_DisplayNameField;
    [SerializeField] private RelayConnectionManager m_RelayConnectionManager;

    public UnityEvent OnNameNotSetWhenPlayPressed;

    private void Awake()
    {
        m_CreateGameButton.onClick.AddListener(async () =>
        {
            if (string.IsNullOrEmpty(m_DisplayNameField.text) || 
                string.IsNullOrWhiteSpace(m_DisplayNameField.text))
            {
                OnNameNotSetWhenPlayPressed?.Invoke();
                return;
            }
            await m_RelayConnectionManager.CreateRelay();
        });

        m_JoinGameButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(m_DisplayNameField.text) ||
                string.IsNullOrWhiteSpace(m_DisplayNameField.text))
            {
                OnNameNotSetWhenPlayPressed?.Invoke();
                return;
            }
            m_RelayConnectionManager.JoinRelay(m_JoinCodeField.text);
        });

        m_QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        m_DisplayNameField.onEndEdit.AddListener((displayName) =>
        {
            PlayerProfileData playerProfile = new();
            PlayerProfileSerialiser.TryLoadProfileData(out playerProfile);
            playerProfile.DisplayName = displayName;
            PlayerProfileSerialiser.SaveProfileData(playerProfile);
        });

        if (PlayerProfileSerialiser.TryLoadProfileData(out PlayerProfileData profile))
        {
            m_DisplayNameField.text = profile.DisplayName;
        }
    }
}
