using Unity.Netcode;
using UnityEngine;


namespace GDS
{
    public class PickUp : NetworkBehaviour
    {
        public void Attach(NetworkObject target)
        {
            if (target == null)
                return;

            NetworkObjectReference targetRef = target;

            AttachRpc(targetRef);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void AttachRpc(NetworkObjectReference targetRef)
        {
            if (!targetRef.TryGet(out NetworkObject target))
                return;

            NetworkObject.TrySetParent(target);
        }
    }
}
