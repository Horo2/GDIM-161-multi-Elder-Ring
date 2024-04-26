using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class PlayerManager : CharacterManager
    {
        PlayerLocomotionManager playerLocomotionManager;
        protected override void Awake()
        {
            base.Awake();

            // Do more stuff, only for the Player

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
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
    }
}
