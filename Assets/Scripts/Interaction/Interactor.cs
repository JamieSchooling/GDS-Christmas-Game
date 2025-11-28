using UnityEngine;

namespace GDS
{
    /// <summary>
    /// Provides functionality to detect and interact with objects within a specified radius in the game world.
    /// </summary>
    /// <remarks>Detects objects within a radius, filtered by a <see cref="LayerMask"/>. When an <see cref="Interactable"/> is detected
    /// and the interact button is pressed, the object is interacted with.</remarks>
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private float m_InteractRadius = 2.5f;
        [SerializeField] private LayerMask m_InteractableLayers;

        private InputSystem_Actions m_Input;

        private void Awake()
        {
            m_Input = new();
            m_Input.Enable();
        }

        void Update()
        {
            Collider2D collider =
                Physics2D.OverlapCircle(transform.position, m_InteractRadius, m_InteractableLayers);
            if (!collider) return;

            if (collider.TryGetComponent(out Interactable interactable)
                && m_Input.Player.Interact.WasPressedThisFrame())
            {
                interactable.Interact();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_InteractRadius);
        }
    }
}