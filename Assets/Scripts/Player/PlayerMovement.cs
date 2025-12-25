using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


namespace GDS
{
    /// <summary>
    /// Controls the movement of a player character.
    /// </summary>
    [RequireComponent(typeof(NetworkRigidbody2D))]
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float m_MoveSpeed = 3.0f;
        private Rigidbody2D m_Rigidbody;

        private Vector2 m_InputDirection;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
        }

        public void UpdateInput(PlayerInput input)
        {
            m_InputDirection = input.Direction;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            m_Rigidbody.linearVelocity = m_MoveSpeed * m_InputDirection;
        }
    }
}
