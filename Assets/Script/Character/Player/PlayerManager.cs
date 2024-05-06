using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Horo
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;

        protected override void Awake()
        {
            base.Awake();

            // Do more stuff, only for the Player
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
        }

        protected override void Update()
        {
            base.Update();

            //If we do not own this gameobejct, we do not control or edit it.
            if (!IsOwner)
                return;
            // Handle movement
            playerLocomotionManager.HandleAllMovement();

            //Regen stamina
            playerStatsManager.RegencerateStamina();
        }

        protected override void LateUpdate()
        {
            if(!IsOwner) 
                return;

            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions();
        }

        //����Ҫ���ã���netcode�Դ���funtion��������update��������ֵ���ȥ�ͻ�����߼�ʵʱ����
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            //If this the player obejct owned by this clent
            if(IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;

                //�����������仯ʱ��ʵʱ��ӳ������UI��,���õķ�����Ҫ�Դ����ñ��� Old Value�� New Value
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRgenTimer;

                // This will be moved when saving and loading is added
                playerNetworkManager.maxStamina.Value = playerStatsManager.CalcuateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
                playerNetworkManager.currentStamina.Value = playerStatsManager.CalcuateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
                PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
                
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

           currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;           
            currentCharacterData.zPosition = transform.position.z;
        }

        public void LoadGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPosition;
            
        }
    }
}
