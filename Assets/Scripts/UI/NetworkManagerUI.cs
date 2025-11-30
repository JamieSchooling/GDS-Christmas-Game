using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button m_HostButton;
    [SerializeField] private Button m_ClientButton;

    private void Awake()
    {
        m_HostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        m_ClientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
