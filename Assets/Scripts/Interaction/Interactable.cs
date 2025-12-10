using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace GDS
{
    /// <summary>
    /// Represents an object that can be interacted with, triggering a <see cref="UnityEvent"/>.
    /// </summary>
    public class Interactable : NetworkBehaviour
    {
        public UnityEvent<NetworkObject> OnInteract;

        /// <summary>
        /// Triggers the OnInteract event
        /// </summary>
        public void Interact(NetworkObject networkObject)
        {
            OnInteract?.Invoke(networkObject);
        }
    }
}