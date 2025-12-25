using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    /// <summary>
    /// Provides functionality to detect and interact with objects within a specified radius in the game world.
    /// </summary>
    /// <remarks>Detects objects within a radius, filtered by a <see cref="LayerMask"/>. When an <see cref="InteractableNetworked"/> is detected
    /// and the interact button is pressed, the object is interacted with.</remarks>
    [RequireComponent(typeof(NetworkObject))]
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] private float m_InteractRadius = 2.5f;
        [SerializeField] private LayerMask m_InteractableLayers;

        private bool m_InteractPressed;

        public void UpdateInput(PlayerInput input)
        {
            m_InteractPressed = input.InteractPressedThisFrame;
        }

        public void CheckForInteractables()
        {
            Collider2D collider =
                Physics2D.OverlapCircle(transform.position, m_InteractRadius, m_InteractableLayers);
            if (!collider) return;

            if (collider.TryGetComponent(out Interactable interactable)
                && m_InteractPressed)
            {
                interactable.Interact(gameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_InteractRadius);
        }
    }
}