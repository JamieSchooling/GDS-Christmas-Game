using System;
using Unity.Netcode;
using UnityEngine;

namespace GDS
{
    public struct PlayerInput
    {
        public Vector2 Direction;
        public bool InteractPressedThisFrame;
    }

    public class Player : NetworkBehaviour
    {
        [SerializeField] private PlayerMovement m_PlayerMovement;
        [SerializeField] private InteractorNetworked m_PlayerInteractor;

        private InputSystem_Actions m_Input;

        private PlayerInput m_CurrentInput;

        public Transform Body => m_PlayerMovement.transform;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            m_Input = new();
            m_Input.Enable();

            m_PlayerInteractor.Initialise(this);
        }

        private void Update()
        {
            if (!IsOwner) return;

            m_CurrentInput.Direction = m_Input.Player.Move.ReadValue<Vector2>();
            m_CurrentInput.InteractPressedThisFrame = m_Input.Player.Interact.WasPressedThisFrame();

            m_PlayerMovement.UpdateInput(m_CurrentInput);
            m_PlayerInteractor.UpdateInput(m_CurrentInput);

            m_PlayerInteractor.CheckForInteractables();
        }
    }
}
