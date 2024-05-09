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
                //�� 1. Find a way to read the values of a joy stick��
                //PlayerMovement.Movement��һ��������
                //����ҽ�����������������ƶ���Ϸҡ�ˣ�ʱ��performed�¼��ͻᱻ������
                //"playerControls.PlayerMovement.Movement.performed += i =>" �ⲿ�����¼����ġ�
                //��ʾ���� Movement ������ִ��ʱ��ִ���ұߵĴ��롱��
                //Lambda ���ʽ��"i => MovementInput = i.ReadValue<Vector2>()" ��һ��Lambda���ʽ������һ������������
                //�������˵��¼�����ʱӦ��ִ�еľ����ж�����������ʽ�У�"i"���¼��������ģ������˸��¼�������������ݡ�
                    //"i.ReadValue<Vector2>()" �Ǵ��������������ȡ�������������
                    //"MovementInput = i.ReadValue<Vector2>()" �������ά������ֵ�� MovementInput
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
        // ����Ұ������������ֵ��ȡ���洢,����Դ˸���ˮƽ�ʹ�ֱ�������
        private void HandlePlayerMovementInput()
        {
            // ��ȡ��ҵĴ�ֱ��ˮƽ����
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            // ��������ǿ�ȣ��޶���0.5��1֮���Լ��ٶȼ���
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

