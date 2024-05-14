using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Horo
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        PlayerManager player;
        public WeaponModelInstantiationSlot rightHandSlot;
        public WeaponModelInstantiationSlot leftHandSlot;

        [SerializeField] WeaponManager rightWeaponManager;
        [SerializeField] WeaponManager leftWeaponManager;

        public GameObject rightHandWeaponModel;
        public GameObject leftHandWeaponModel;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();

            // Get our slots
            InitalizeWeaponSlots();
        }

        protected override void Start()
        {
            base.Start();

            LoadWeaponsOnBothHands();
        }
        private void InitalizeWeaponSlots()
        {
            WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

            foreach(var weaponSlot in weaponSlots)
            {
                if(weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
                {
                    rightHandSlot = weaponSlot;
                }
                else if(weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
                {
                    leftHandSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponsOnBothHands()
        {
            LoadRightWeapon();
            LoadLeftWeapon();
        }

        // Right Weapon
        public void SwitchRightWeapon()
        {           
            if (!player.IsOwner)
                return;

            player.playerAnimatorManager.PlayerTargetActionAnimation("Swap_Right_Weapon_01", false,true,true,true);


            // 1. Check if we hav4e another wepon besides our  main weapon, if we do, NEVER swap to unarmed, rotate between weapon 1 and 2
            // 2. If we dont, swap to unarmed, then SKIP the other empty slot and swap back. Do not process both empth slots before returning to main weapon

            WeaponItem selectedWeapon = null;

            // Disable two handing if we are two handing

            // Add one to rou index to switch to the next potential wepon
            player.playerInventoryManager.rightHandWeaponIndex += 1;

            /// If our Index is out of bounds, reset it to position #1(0)
            if(player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
            {
                player.playerInventoryManager.rightHandWeaponIndex = 0;

                // We check if we are holding more than one weapon
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;

                for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;

                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCount <= 1)
                {
                    player.playerInventoryManager.rightHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
                }

                return;
            }

            foreach(WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
            {
                // If the next potential weapon does not equal the unarmed  weapon
                if (player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID !=  WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
                    // Assign the network weapon ID so it siwtched for all connected clients
                    player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;
                    return;
                }
            }

            if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
            {
                SwitchRightWeapon();
            }

        }

        public void LoadRightWeapon()
        {
            if(player.playerInventoryManager.currentRightHandWeapon != null)
            {
                //Remove the old weapon
                rightHandSlot.UnloadWeapon();

                //Bring in the new weapon
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                rightWeaponManager.SetWeaponDamage(player,player.playerInventoryManager.currentRightHandWeapon); // 确保武器模型加载后，将武器的伤害属性应用到执行伤害的collider上
                // Assign weapons damage, to its collider
            }
        }

        // Left Weapon
        public void SwitchLeftWeapon()
        {

        }
        
        public void LoadLeftWeapon()
        {
            if (player.playerInventoryManager.currentLeftHandWeapon != null)
            {
                //Remove the old weapon
                leftHandSlot.UnloadWeapon();

                //Bring in the new weapon
                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon); // 确保武器模型加载后，将武器的伤害属性应用到执行伤害的collider上
                // Assign weapons damage, to its collider
            }
        }
    }
}
