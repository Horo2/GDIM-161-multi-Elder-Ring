using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;
        // those values only taking from PlayerInputManager Class
        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2;
        [SerializeField] float runningSpeed = 5;
        [SerializeField] float rotationSpeed = 15;

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
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
                // if locked on, pass HORZ and VERT
            }
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
            // Grounde Movement
            // Aerial Movement
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
            GetMovementValues();

            //角色的移动方向是基于相机的观看方向和玩家的输入
            // Our move direction is based on our cameras facing persprective & our movemnt inputs
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize(); // Normalize()的作用是将向量值锁定到最大为1（整数）,归1
            moveDirection.y = 0;

            if(PlayerInputManager.instance.moveAmount > 0.5f)
            {
                //Move at a running speed
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if(PlayerInputManager.instance.moveAmount >=0.5f)
            {
                //Move at walking speed
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

        // 处理角色的旋转，使角色朝向由玩家输入决定的方向。
        private void HandleRotation()
        {
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
    }
}
