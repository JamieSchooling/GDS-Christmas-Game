using Unity.Netcode;
using UnityEngine;

public class TargetFollow : NetworkBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private Vector2 m_Offset = Vector2.zero;

    public void SetTarget(Transform target)
    {
        m_Target = target;
    }

    private void LateUpdate()
    {
        if (!IsOwner || m_Target == null) return;

        transform.position = m_Target.position + (Vector3)m_Offset;
    }
}
