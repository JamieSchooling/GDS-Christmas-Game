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

        private InputSystem_Actions m_Input;
        private Rigidbody2D m_Rigidbody;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();

            m_Input = new();
            m_Input.Enable();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!IsOwner) return;

            m_Rigidbody.linearVelocity = m_MoveSpeed * m_Input.Player.Move.ReadValue<Vector2>();
        }
    }
}
