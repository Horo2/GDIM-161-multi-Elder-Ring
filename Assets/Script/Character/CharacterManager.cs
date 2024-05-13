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
                //SmoothDamp需要四个值，第一个是目前位置，第二个是预计位置，第三个移动速度第四个是平滑程度。
                //用于让一个坐标的物体平滑移动到另一个坐标，而不是瞬移
                //当前位置 (transform.position)：这是物体在本地客户端上的当前实际位置。
                //目标位置(characterNetworkManager.networkPosition.Value)：这是从网络（可能是从服务器或其他客户端）同步来的物体的位置。
                //目标位置代表了物体应该处于的位置
                // 通过“ref characterNetworkManager.networkPositionVelocity” 作为速度变量，
                // 还能根据前后两帧之间的变化自适应地调整速度，进一步优化运动的平滑性。

                // Position
                transform.position = Vector3.SmoothDamp
                    (transform.position, characterNetworkManager.networkPosition.Value,
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionSmoothTime);

                //类似于“Vector3.SmoothDamp”，用于平滑转向
                //第一个值是旋转的初始四元数，第二个是旋转的结束四元数，第三个是平滑程度

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

