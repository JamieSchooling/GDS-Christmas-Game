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
    [SerializeField] private GameConnectionManager m_GameConnectionManager
        ;

    private void Awake()
    {
        m_CreateGameButton.onClick.AddListener(async () =>
        {
            await m_GameConnectionManager.CreateRelay();
        });

        m_JoinGameButton.onClick.AddListener(() =>
        {
            m_GameConnectionManager.JoinRelay(m_JoinCodeField.text);
        });

        m_QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
