using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Horo
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;

        public PlayerManager player;

        PlayerControls playerControls;
      
        [Header("CAMERA MOVEMENT INPUT")]
        [SerializeField] private Vector2 cameraInput;
        public float cameraHorizontalInput;
        public float cameraVerticalInput;

        [Header("PLAVER MOVEMENT INPUT")]
        [SerializeField] private Vector2 movementInput;
        public float horizontalInput;
        public float verticalInput;
        public float moveAmount;

        [Header("PLAYER ACTION INPUT")]
        [SerializeField] bool dodgeInput = false;
        [SerializeField] bool sprintInput = false;
        [SerializeField] bool jumpInput = false;

        private void Awake()
        {
            if (instance == null) { instance = this; }
            else { Destroy(gameObject); }           
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            //When the Scene Changes, run this logic
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;          
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            //If we are loading into our world Scene, Enable our players controls
            if(newScene.buildIndex == WorldSaveGameManager.instance.GetWoroldSceneIndex())
            {
                instance.enabled = true;
            }
            //Otherwise we must be at the main menu, disable our palyers controls
            //this is so our palyer cant move around if we enter things like a character creation menu etc...
            else
            {
                instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                //（ 1. Find a way to read the values of a joy stick）
                //PlayerMovement.Movement是一个动作，
                //当玩家进行这个动作（比如移动游戏摇杆）时，performed事件就会被触发。
                //"playerControls.PlayerMovement.Movement.performed += i =>" 这部分是事件订阅。
                //表示“当 Movement 动作被执行时，执行右边的代码”。
                //Lambda 表达式："i => MovementInput = i.ReadValue<Vector2>()" 是一个Lambda表达式，这是一个匿名函数，
                //它定义了当事件发生时应该执行的具体行动。在这个表达式中："i"是事件的上下文，包含了该事件的所有相关数据。
                    //"i.ReadValue<Vector2>()" 是从这个上下文中提取具体的输入数据
                    //"MovementInput = i.ReadValue<Vector2>()" 将这个二维向量赋值给 MovementInput
                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
                playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

                // Holiding the input, activates the bool to true
                playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
                // Releasing the input, sets the bool to false
                playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            //if we destroy this object, unsubscribe form this event
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        //When the game is minimize or lower the window, sotp adjusting inputs
        private void OnApplicationFocus(bool focus)
        {
            if(enabled)
            {
                if(focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSpringtingInput();
        }

        //MOVEMENT
        // 将玩家按按键所输入的值读取并存储,随后以此更新水平和垂直输入变量
        private void HandlePlayerMovementInput()
        {
            // 提取玩家的垂直和水平输入
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            // 计算输入强度，限定在0.5和1之间以简化速度级别
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            //We clamp the values, so they are 0, 0.5, 1(Optional)
            if(moveAmount <=0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if(moveAmount >0.5 && moveAmount <=1)
            {
                moveAmount = 1;
            }

            //Why do we pass 0 on the horizontal? because we only want non-strafing movement
            //we use the horizontal when we are strafing or locked on

            if(player == null)
                return;
            //if we are not locked on, only use the move amount
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

            //if we are locked on pass the horizontal movement as well
        }
        private void HandleCameraMovementInput() 
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }

        //ACTIONS
        private void HandleDodgeInput()
        {
            if(dodgeInput)
            {
                dodgeInput = false;

                // Future note: return (Do nothing) if menu or ui window is open

                //perform a dodge
                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void HandleSpringtingInput()
        {
            if(sprintInput)
            {
                // HANDLE SPRINTING
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void HandleJumpInput()
        {
            if(jumpInput)
            {
                jumpInput = false;

                // If we have a UI window open, simply return without doing anything

                // Attemp to perform jump
                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }

    }
}

