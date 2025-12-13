using UnityEngine;
using UnityEngine.SceneManagement;

namespace GDS 
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private SceneObject m_Scene;

        public void LoadScene()
        {
            SceneManager.LoadSceneAsync(m_Scene);
        }

        public void LoadScene(SceneObject scene)
        {
            SceneManager.LoadSceneAsync(scene);
        }
    }
}
