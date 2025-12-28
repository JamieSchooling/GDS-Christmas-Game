using UnityEngine;


namespace GDS
{
    public class PlayerSpriteAssigner : MonoBehaviour
    {
        [SerializeField] private Sprite[] m_SpriteSheets;
        [SerializeField] private Animator m_Animator;

        public void Initialise(ulong clientID)
        {
            if ((int)clientID >= m_SpriteSheets.Length)
            {
                Debug.LogError($"Couldn't assign sprite to client: {clientID}");
                return;
            }

            m_Animator.SetSpriteSheet(m_SpriteSheets[clientID]);
        }
    }
}