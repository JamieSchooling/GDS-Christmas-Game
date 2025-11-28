using UnityEngine;
using UnityEngine.Events;

namespace GDS
{
    /// <summary>
    /// Represents an object that can be interacted with, triggering a <see cref="UnityEvent"/>.
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        public UnityEvent OnInteract;

        /// <summary>
        /// Triggers the OnInteract event
        /// </summary>
        public void Interact()
        {
            OnInteract?.Invoke();
        }
    }
}