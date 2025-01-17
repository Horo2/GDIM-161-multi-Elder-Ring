using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Horo
{
    public class PlayerManager : CharacterManager
    {
        [Header("DEBUG MENU")]
        [SerializeField] bool respawnCharacter = false;
        [SerializeField] bool switchRightWeapon = false;

        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;

        protected override void Awake()
        {
            base.Awake();

            // Do more stuff, only for the Player
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
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

            DebugMenu();
        }

        protected override void LateUpdate()
        {
            if(!IsOwner) 
                return;

            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions();
        }

        //不需要引用，是netcode自带的funtion，类似于update，变量数值填进去就会根据逻辑实时更新
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            //If this the player obejct owned by this clent
            if(IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;

                //这里指的是当+=前的数值发生变化时，执行+=后面的method
                // Update the total amount of Health or Stamina when the stat linked to either changes
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

                //当耐力发生变化时，实时反映到耐力UI上,调用的方法需要自带设置变量 Old Value和 New Value
                // Updates UI stat bars when a stat changes(Helath or Stamina)
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRgenTimer;

              
                
            }

            //STATS
            //这里指的是当+=前的数值发生变化时，执行+=后面的method
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

            // EQUIPMENT
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;

            // Upon connecting, if we are the owner of this character, but we are not the server, reload our character data to this newly instantiated character
            // We dont run this if we are the server, because since they are the host, they are already loaded in and don't need to reload their data
            if(IsOwner && !IsServer)
            {
                LoadGameDataToCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
            }
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
            }
           
            return base.ProcessDeathEvent(manuallySelectDeathAnimation);

            // Check for players that are alive, if 0 respawn characters
        }

        public override void ReviveCharacter()
        {
            base.ReviveCharacter();

            if(IsOwner)
            {
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
                // Restore focus points

                // Player Rebirth effects
                playerAnimatorManager.PlayerTargetActionAnimation("Empty", false);
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;           
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamina =playerNetworkManager.currentStamina.Value;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;
        }

        public void LoadGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPosition;

            playerNetworkManager.vitality.Value = currentCharacterData.vitality;
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;

            // This will be moved when saving and loading is added
            playerNetworkManager.maxHealth.Value = playerStatsManager.CalcuateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalcuateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }

        // DEBUG DELETE LATER
        private void DebugMenu()
        {
            if(respawnCharacter)
            {
                respawnCharacter = false;
                ReviveCharacter();
            }
            if(switchRightWeapon)
            {
                switchRightWeapon = false;
                playerEquipmentManager.SwitchRightWeapon();

            }
        }
    }
}
