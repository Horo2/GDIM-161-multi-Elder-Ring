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

            //��ɫ���ƶ������ǻ�������Ĺۿ��������ҵ�����
            // Our move direction is based on our cameras facing persprective & our movemnt inputs
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize(); // Normalize()�������ǽ�����ֵ���������Ϊ1��������,��1
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

        // �����ɫ����ת��ʹ��ɫ�����������������ķ���
        private void HandleRotation()
        {
            // ��ʼ��Ŀ����ת����Ϊ��������
            targetRotationDirection = Vector3.zero;
            // ���ݴ�ֱ���루ǰ���ƶ���������Ŀ����ת����Ϊ�����ǰ����
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            // ����ˮƽ���루�����ƶ�������������ҷ�����ӵ�Ŀ����ת�����С�
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;
            // ���Ŀ����ת����Ϊ�㣨û�����룩����Ĭ��ʹ�õ�ǰ�����ǰ����
            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }
            // ����һ����ת��ʹ������Ŀ����ת����
            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            // ʹ�����β�ֵ��Slerp��ƽ���ع��ɵ�ǰ��ת������ת��
            // rotationSpeed ������ת�ٶȣ�Time.deltaTime ȷ����תƽ������֡���޹ء�
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            // Ӧ�ü������Ŀ����ת������� Transform �ϡ�
            transform.rotation = targetRotation;

        }
    }
}
