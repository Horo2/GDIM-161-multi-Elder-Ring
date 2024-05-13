using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

namespace Horo
{   
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public CharacterController characterController;
        [HideInInspector] public Animator animator;

        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectManager characterEffectManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;

        [Header("Flags")]
        public bool isPerformingAction = false;
        public bool isJumping = false;
        public bool isGrounded = false;
        public bool applyRootMotion = false;
        public bool canRotate = true;
        public bool canMove = true;

        


       protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectManager = GetComponent<CharacterEffectManager>();
            characterAnimatorManager = GetComponent <CharacterAnimatorManager>();
        }

        protected virtual void Update()
        {
            animator.SetBool("IsGrounded",isGrounded);

            // If this character is being controlled from our side, then assign its network position to the position of our transform
            if(IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            //if this character is being controlled from else where, then assign its position here locally by the position of its network transform
            else
            {
                //SmoothDamp��Ҫ�ĸ�ֵ����һ����Ŀǰλ�ã��ڶ�����Ԥ��λ�ã��������ƶ��ٶȵ��ĸ���ƽ���̶ȡ�
                //������һ�����������ƽ���ƶ�����һ�����꣬������˲��
                //��ǰλ�� (transform.position)�����������ڱ��ؿͻ����ϵĵ�ǰʵ��λ�á�
                //Ŀ��λ��(characterNetworkManager.networkPosition.Value)�����Ǵ����磨�����Ǵӷ������������ͻ��ˣ�ͬ�����������λ�á�
                //Ŀ��λ�ô���������Ӧ�ô��ڵ�λ��
                // ͨ����ref characterNetworkManager.networkPositionVelocity�� ��Ϊ�ٶȱ�����
                // ���ܸ���ǰ����֮֡��ı仯����Ӧ�ص����ٶȣ���һ���Ż��˶���ƽ���ԡ�

                // Position
                transform.position = Vector3.SmoothDamp
                    (transform.position, characterNetworkManager.networkPosition.Value,
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionSmoothTime);

                //�����ڡ�Vector3.SmoothDamp��������ƽ��ת��
                //��һ��ֵ����ת�ĳ�ʼ��Ԫ�����ڶ�������ת�Ľ�����Ԫ������������ƽ���̶�

                // Rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    characterNetworkManager.networkRotation.Value, 
                    characterNetworkManager.networkRotationSmoothTime);
            }
        }

        protected virtual void LateUpdate()
        {

        }

        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if(IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;

                // Reset any flags here that need to be reset
                // Nothing yet

                // If we are not grounded, play an aerial death animation

                if(!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayerTargetActionAnimation("Dead_01", true);
                }
            }

            // Play some death SFX

            yield return new WaitForSeconds(5);

            // Award players with runes

            // Disable character

        }

        public virtual void ReviveCharacter()
        {

        }

        
    }
}

