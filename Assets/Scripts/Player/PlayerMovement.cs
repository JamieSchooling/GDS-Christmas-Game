using UnityEngine;


namespace GDS
{
    /// <summary>
    /// Controls the movement of a player character.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float m_MoveSpeed = 3.0f;

        private InputSystem_Actions m_Input;

        private void Awake()
        {
            m_Input = new();
            m_Input.Enable();
        }

        // Update is called once per frame
        private void Update()
        {
            transform.position += m_MoveSpeed * Time.deltaTime * (Vector3)m_Input.Player.Move.ReadValue<Vector2>();
        }
    }
}
