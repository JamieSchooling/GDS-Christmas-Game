using GDS;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button m_CreateGameButton;
    [SerializeField] private Button m_JoinGameButton;
    [SerializeField] private Button m_QuitButton;
    [SerializeField] private TMP_InputField m_JoinCodeField;
    [SerializeField] private RelayConnectionManager m_RelayConnectionManager
        ;

    private void Awake()
    {
        m_CreateGameButton.onClick.AddListener(async () =>
        {
            await m_RelayConnectionManager.CreateRelay();
        });

        m_JoinGameButton.onClick.AddListener(() =>
        {
            m_RelayConnectionManager.JoinRelay(m_JoinCodeField.text);
        });

        m_QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
