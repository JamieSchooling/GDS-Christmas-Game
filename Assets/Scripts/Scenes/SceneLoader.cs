using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GDS 
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private SceneObject m_Scene;

        public void LoadSceneLocal()
        {
            SceneManager.LoadSceneAsync(m_Scene);
        }

        public void LoadSceneLocal(SceneObject scene)
        {
            SceneManager.LoadSceneAsync(scene);
        }

        public void LoadSceneNetworked()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            NetworkManager.Singleton.SceneManager.LoadScene(m_Scene, LoadSceneMode.Single);
        }

        public void LoadSceneNetworked(SceneObject scene)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
    }
}
