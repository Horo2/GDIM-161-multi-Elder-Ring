using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;
        // those values only taking from PlayerInputManager Class
        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2;
        [SerializeField] float runningSpeed = 5;
        [SerializeField] float sprintingSpeed = 10;
        [SerializeField] float rotationSpeed = 15;
        [SerializeField] int sprintingStaminaCost = 2;

        [Header("Jump")]
        [SerializeField] float jumpStaminaCoust = 25;
        [SerializeField] float jumpHeight = 4;
        [SerializeField] float jumpForwardSpeed = 5;
        [SerializeField] float freeFallSpeed = 2;
        private Vector3 jumpDirection;

        [Header("Dodge")]
        private Vector3 rollDirection;
        [SerializeField] float dodgeStaminaCoust = 25;
        

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();
            if(player.IsOwner)
            {
                player.characterNetworkManager.verticalMovement.Value = verticalMovement;
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
                player.characterNetworkManager.moveAmount.Value = moveAmount;
            }
            else
            {
                verticalMovement = player.characterNetworkManager.verticalMovement.Value;
                horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
                moveAmount = player.characterNetworkManager.moveAmount.Value;

                //If not locked on, pass move amount
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
                // if locked on, pass HORZ and VERT
            }
        }

        public void HandleAllMovement()
        {
            // Grounde Movement
            HandleGroundedMovement();
            HandleRotation();
            HandleJumpingMovement();
            HandleFreeFallMovement();
        }
       
        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;
            //Clamp the movements
        }

        private void HandleGroundedMovement()
        {
            if (!player.canMove)
                return;

            GetMovementValues();
           
            //角色的移动方向是基于相机的观看方向和玩家的输入
            // Our move direction is based on our cameras facing persprective & our movemnt inputs
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize(); // Normalize()的作用是将向量值锁定到最大为1（整数）,归1
            moveDirection.y = 0;

            if(player.playerNetworkManager.isSprinting.Value)
            {
                player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            }
            else
            {
                if (PlayerInputManager.instance.moveAmount > 0.5f)
                {
                    //Move at a running speed
                    player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (PlayerInputManager.instance.moveAmount >= 0.5f)
                {
                    //Move at walking speed
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }           
        }

        private void HandleJumpingMovement()
        {
            if(player.isJumping)
            {
                player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
            }
        }

        private void HandleFreeFallMovement()
        {
            if(!player.isGrounded)
            {
                Vector3 freeFallDirection;

                freeFallDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                freeFallDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
                freeFallDirection.y = 0;

                player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
            }
        }

        // 处理角色的旋转，使角色朝向由玩家输入决定的方向。
        private void HandleRotation()
        {
            if(!player.canRotate)
                return;

            // 初始化目标旋转方向为零向量。
            targetRotationDirection = Vector3.zero;
            // 根据垂直输入（前后移动），设置目标旋转方向为相机的前方。
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            // 根据水平输入（左右移动），将相机的右方向添加到目标旋转方向中。
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;
            // 如果目标旋转方向为零（没有输入），则默认使用当前对象的前方向。
            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }
            // 创建一个旋转，使对象朝向目标旋转方向。
            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            // 使用球形插值（Slerp）平滑地过渡当前旋转到新旋转。
            // rotationSpeed 控制旋转速度，Time.deltaTime 确保旋转平滑且与帧率无关。
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            // 应用计算出的目标旋转到对象的 Transform 上。
            transform.rotation = targetRotation;

        }

        public void HandleSprinting()
        {
            if(player.isPerformingAction)
            {
                // Set sprinting to false
                player.playerNetworkManager.isSprinting.Value = false;
            }

            // If we are out of stamina, set sprinting to false
            if(player.playerNetworkManager.currentStamina.Value <=0)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }

            // If we are moving , Sprinting is true
            if(moveAmount >= 0.5)
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            // If we are stationary/moving slowly sprinting is false
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if(player.playerNetworkManager.isSprinting.Value)
            {
                player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
            }
        }

        public void AttemptToPerformDodge()
        {
            if (player.isPerformingAction)
                return;

            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return;
            // If we are moveingwhen we attenpt to dodge, wen perform a roll
            if(PlayerInputManager.instance.moveAmount > 0)
            {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;

                // Perform a roll animation
                player.playerAnimatorManager.PlayerTargetActionAnimation("Roll_Forward_01", true, true);
            }
            // If we are statinoary, we perform a backstep
            else
            {
                //Perform a backstep animation
                player.playerAnimatorManager.PlayerTargetActionAnimation("Back_Step_01", true, true);
            }

            player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCoust;


        }

        public void AttemptToPerformJump()
        {
            // If we are performing an general action, we do not want to allow a jump(will change when combat is adde)
            if (player.isPerformingAction)
                return;

            // If we are out of stamina, we do not wish to allow a jump
            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return;

            // If we are already in a jump, we do not want to allow a jump again until the current jump has finished
            if (player.isJumping)
                return;

            //if we are not grounded, we do not want to allow a jump
            if (!player.isGrounded)
                return;

            // If we are two handing our weapon, play the two handed jump animation, otherwise play the one handed animation (TO DO)
            player.playerAnimatorManager.PlayerTargetActionAnimation("Main_Jump_01", false);

            player.isJumping = true;

            player.playerNetworkManager.currentStamina.Value -= jumpStaminaCoust;

            jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
            jumpDirection.y = 0;

            if(jumpDirection != Vector3.zero)
            {
                // If we are sprinting jump direction is at full distance
                if (player.playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= 1;
                }
                // If we are running jump direction is at half distance
                else if (PlayerInputManager.instance.moveAmount > 0.5)
                {
                    jumpDirection *= 0.5f;
                }
                // If we are walking jump direction is at quarter distance
                else if (PlayerInputManager.instance.moveAmount <= 0.5)
                {
                    jumpDirection *= 0.25f;
                }
            }
            
        }

        public void ApplyJumpingVelocity()
        {
            // Apply an upward velocity depending on forces in our game
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }

    }
}
