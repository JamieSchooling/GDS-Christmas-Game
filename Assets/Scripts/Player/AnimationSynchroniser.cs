using Unity.Netcode;
using UnityEngine;


namespace GDS
{
    public enum AnimationState
    {
        Up, Down, Left, Right
    }

    public class AnimationSynchroniser : NetworkBehaviour
    {
        [SerializeField] private Sprite[] m_SpriteSheets;
        [SerializeField] private Animator m_Animator;

        private NetworkVariable<AnimationState> m_AnimState = new(
            readPerm: NetworkVariableReadPermission.Everyone, 
            writePerm: NetworkVariableWritePermission.Owner
        );

        public void Initialise(ulong clientID)
        {
            if ((int)clientID >= m_SpriteSheets.Length)
            {
                Debug.LogError($"Couldn't assign sprite to client: {clientID}");
                return;
            }

            m_Animator.SetSpriteSheet(m_SpriteSheets[clientID]);
        }

        // Update is called once per frame
        public void UpdateAnimations(PlayerInput input)
        {
            if (IsOwner)
            {
                if (input.Direction.x > 0)
                    m_AnimState.Value = AnimationState.Right;
                else if (input.Direction.x < 0)
                    m_AnimState.Value = AnimationState.Left;
                else if (input.Direction.y > 0)
                    m_AnimState.Value = AnimationState.Up;
                else if (input.Direction.y < 0)
                    m_AnimState.Value = AnimationState.Down;
            }

            switch (m_AnimState.Value)
            {
                case AnimationState.Up:
                    m_Animator.Play("Up"); break;
                case AnimationState.Down:
                    m_Animator.Play("Down"); break;
                case AnimationState.Left:
                    m_Animator.Play("Left"); break;
                case AnimationState.Right:
                    m_Animator.Play("Right"); break;

            }
        }
    }
}