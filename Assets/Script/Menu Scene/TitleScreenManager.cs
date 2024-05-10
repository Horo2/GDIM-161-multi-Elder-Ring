using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


namespace Horo
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;
        [Header("Menu")]
        [SerializeField] GameObject titleScreenMainMenu;
        [SerializeField] GameObject titleScreenLoadMenu;

        [Header("Buttons")]       
        [SerializeField] Button LoadMenuReturnButton;
        [SerializeField] Button MainMenuLoadGameButton;
        [SerializeField] Button mainMenuNewGameButton;
        [SerializeField] Button deleteCharacterPopUpConfirmButton;

        [Header("Pop Ups")]
        [SerializeField] GameObject noCharacterSlotsPopUp;
        [SerializeField] Button noCharacterSlotsOkayButton;
        [SerializeField] GameObject deleteCharacterSlotPopUp;

        [Header("Character Slot")]
        public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;


        [Header("Title Screen Inputs")]
        [SerializeField] bool deleteCharacterSlot = false;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
            
        }

        public void OpenLoadGameMenu()
        {
            // Close main menu
            titleScreenMainMenu.SetActive(false); 

            // Open Load menu
            titleScreenLoadMenu.SetActive(true);

            // Select the return button first
            LoadMenuReturnButton.Select();
        }

        public void CloseLoadGameMenu()
        {
            // Close Load menu
            titleScreenLoadMenu.SetActive(false);

            // Open main menu
            titleScreenMainMenu.SetActive(true);

            // Selected the Load button
            MainMenuLoadGameButton.Select();
        }

        public void DisplayNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPopUp.SetActive(true);
            noCharacterSlotsOkayButton.Select();
        }

        public void CloseNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPopUp.SetActive(false);
            mainMenuNewGameButton.Select();


        }

        //Character Slots
        //在UI_Character_Save_Slot 选中的 Save slot 需要同步到TitleScreenManager中，防止出错，也用于删除存档时等操作时用
        public void SelecteCharacterSlot(CharacterSlot characterSlot)
        {
            currentSelectedSlot = characterSlot;
        }

        // 绑定在 return button中，防止出错
        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT;
        }

        //如果目前选择的存档槽位不是NO_SLOT，尝试执行删除存档
        public void AttemptToDeleteCharacterSlot()
        {
            if(currentSelectedSlot != CharacterSlot.NO_SLOT)
            {
                deleteCharacterSlotPopUp.SetActive(true);
                deleteCharacterPopUpConfirmButton.Select();
            }            
        }

        //删除存档的具体逻辑
        public void DeleteCharacterSlot()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);

            // We disable and then enable the load menu, to refresh to slots(the deleted slots will now become inactive)
            titleScreenLoadMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
            LoadMenuReturnButton.Select();
            
        }
        public void CloseDeleteCharacterSlotPopUp()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            LoadMenuReturnButton.Select();
        }

        
    }
}

