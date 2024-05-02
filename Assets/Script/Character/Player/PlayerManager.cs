using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;

        protected override void Awake()
        {
            base.Awake();

            // Do more stuff, only for the Player
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
        }

        protected override void Update()
        {
            base.Update();

            //If we do not own this gameobejct, we do not control or edit it.
            if (!IsOwner)
                return;
            // Handle movement
            playerLocomotionManager.HandleAllMovement();
        }

        protected override void LateUpdate()
        {
            if(!IsOwner) 
                return;

            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            //If this the player obejct owned by this clent
            if(IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
            }
        }
    }
}
