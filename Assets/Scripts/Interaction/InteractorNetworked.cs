using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    /// <summary>
    /// Provides functionality to detect and interact with objects within a specified radius in the game world.
    /// </summary>
    /// <remarks>Detects objects within a radius, filtered by a <see cref="LayerMask"/>. When an <see cref="InteractableNetworked"/> is detected
    /// and the interact button is pressed, the object is interacted with.</remarks>
    public class InteractorNetworked : NetworkBehaviour
    {
        [SerializeField] private float m_InteractRange = 1.0f;
        [SerializeField] private Vector2 m_InteractLocalOrigin = Vector2.zero;
        [SerializeField] private Vector2 m_InteractDirectionalRangeMultiplier = Vector2.zero;
        [SerializeField] private LayerMask m_InteractableLayers;

        private Player m_Owner;
        private bool m_InteractPressed;
        private Vector2 m_InteractLookDirection;
        private Vector2 m_LastInteractLookDirection = Vector2.right;
        private float m_CurrentInteractRange;

        public void Initialise(Player owner)
        {
            m_Owner = owner;

            Vector2 dir = m_LastInteractLookDirection.normalized;
            Vector2 m = m_InteractDirectionalRangeMultiplier;
            float mult = Vector2.Dot(new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y)), m);
            mult = Mathf.Min(mult, Mathf.Max(m.x, m.y));
            m_CurrentInteractRange = m_InteractRange * mult;
        }

        public void UpdateInput(PlayerInput input)
        {
            m_InteractPressed = input.InteractPressedThisFrame;
            m_InteractLookDirection = input.Direction;
            if (m_InteractLookDirection == Vector2.zero)
                m_InteractLookDirection = m_LastInteractLookDirection;

            Vector2 dir = m_InteractLookDirection.normalized;
            Vector2 m = m_InteractDirectionalRangeMultiplier;
            float mult = Mathf.Abs(dir.x) * m.x + Mathf.Abs(dir.y) * m.y;
            mult = Mathf.Min(mult, Mathf.Max(m.x, m.y));
            m_CurrentInteractRange = m_InteractRange * mult;

            m_LastInteractLookDirection = m_InteractLookDirection;
        }

        public void CheckForInteractables()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3)m_InteractLocalOrigin,
                m_InteractLookDirection.normalized,
                m_CurrentInteractRange, m_InteractableLayers);
            
            if (!hit) return;

            if (hit.collider.TryGetComponent(out InteractableNetworked interactable)
                && m_InteractPressed)
            {
                interactable.Interact(m_Owner.NetworkObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 startPos = transform.position + (Vector3)m_InteractLocalOrigin;
            Gizmos.DrawLine(startPos, startPos + (Vector3)m_InteractLookDirection * m_CurrentInteractRange); 
        }
    }
}