using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        CharacterManager character;
        [Header("Ground Check & Jumping")]
        [SerializeField] protected float gravityForce = -5.55f;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundCheckSphereRadius = 1;
        [SerializeField] protected Vector3 yVelocity; // The force at which our character is pulled up or down(Jumping or Falling)
        [SerializeField] protected float groundedYVelocity = -20; // The force at which our character is sticking to the ground whilst they are grounded
        [SerializeField] protected float fallstartYVelocity = -5; // The force at which our character begins to fall when they become ungrounded(Rises as they fall longer)
        protected bool fallingVelocityHAsBeenSet = false;
        protected float inAirTimer = 0;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if(character.isGrounded)
            {
                // If we are not attemping to jump or move upward
                if(yVelocity.y <0)
                {
                    inAirTimer = 0;
                    fallingVelocityHAsBeenSet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                // If wea re not jumping, and our falling velocity has not been set
                if(!character.characterNetworkManager.isJumping.Value && !fallingVelocityHAsBeenSet)
                {
                    fallingVelocityHAsBeenSet = true;
                    yVelocity.y = fallstartYVelocity;
                }
                inAirTimer += Time.deltaTime;
                character.animator.SetFloat("InAirTimer", inAirTimer);

                yVelocity.y += gravityForce * Time.deltaTime;               
                
            }

            // There should always be some force applied to the y velocity
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
        }

        // For testing draws ground check spehre in scene view
        protected void OnDrawGizmosSelected()
        {
            //Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }
    }
}
