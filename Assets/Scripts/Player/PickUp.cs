using Unity.Netcode;
using UnityEngine;


namespace GDS
{
    public class PickUp : NetworkBehaviour
    {
        public void Attach(GameObject gameObject)
        {
            transform.SetParent(gameObject.transform);
        }
    }
}
