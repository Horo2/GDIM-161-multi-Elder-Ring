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
        //Think about goals in steps
        //1. Find a way to read the values of a joy stick
        //2. Move characvter based on those values

        PlayerControls playerControls;

        [SerializeField] private Vector2 MovementInput;
        public float horizontalInput;
        public float verticalInput;
        public float moveAmount;

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
                playerControls.PlayerMovement.Movement.performed += i => MovementInput = i.ReadValue<Vector2>();
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
            HandleMovementInput();
        }

        // ����Ұ������������ֵ��ȡ���洢,����Դ˸���ˮƽ�ʹ�ֱ�������
        private void HandleMovementInput()
        {
            // ��ȡ��ҵĴ�ֱ��ˮƽ����
            verticalInput = MovementInput.y;
            horizontalInput = MovementInput.x;

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
        }

    }
}

