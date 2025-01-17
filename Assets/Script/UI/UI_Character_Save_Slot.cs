using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Horo
{
    public class UI_Character_Save_Slot : MonoBehaviour
    {
        SaveFileDataWriter saveFileWriter;

        [Header("Game slot")]
        public CharacterSlot characterSlot; // 需要在存档槽的UI中，手动选择这个SlotUI属于哪个Slot，例如UI2属于characterSlot_02 etc

        [Header("Character Info")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI timedPlayed;

        private void OnEnable()
        {
            LoadSaveSlots();
        }

        //先判定对应的槽位组件是否有存档，如果有则将角色名字，游玩时间等信息显示到对应的UI组件上，如果没有，则disable对应的 UI gameobject。
        private void LoadSaveSlots()
        {
            saveFileWriter = new SaveFileDataWriter();
            saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;

            // Save slot 01
            if(characterSlot == CharacterSlot.CharacterSlot_01)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if(saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Save slot 02
            else if (characterSlot == CharacterSlot.CharacterSlot_02)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot02.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Save slot 03
            else if (characterSlot == CharacterSlot.CharacterSlot_03)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot03.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Save slot 04
            else if (characterSlot == CharacterSlot.CharacterSlot_04)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot04.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Save slot 05
            else if (characterSlot == CharacterSlot.CharacterSlot_05)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot05.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Save slot 06
            else if (characterSlot == CharacterSlot.CharacterSlot_06)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot06.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Save slot 07
            else if (characterSlot == CharacterSlot.CharacterSlot_07)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot07.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Save slot 08
            else if (characterSlot == CharacterSlot.CharacterSlot_08)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot08.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Save slot 09
            else if (characterSlot == CharacterSlot.CharacterSlot_09)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot09.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Save slot 010
            else if (characterSlot == CharacterSlot.CharacterSlot_010)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // If the file exists, get infomation form it
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot010.characterName;
                }
                // If it does not, disable this gameobject
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        //在 Save Slot (0) 等的 button组件中调用这个method 来判定点击时，要读取哪个槽位
        public void LoadGameFromCharacterSlot()
        {
            
            WorldSaveGameManager.instance.currentChracterSlotBeingUsed = characterSlot;
            WorldSaveGameManager.instance.LoadGame();
        }

        //在 Save Slot (0) 等的 event trigger组件中调用这个method 来判定目前选中的是哪个存档槽
        public void SelectCurrentSlot()
        {
            TitleScreenManager.instance.SelecteCharacterSlot(characterSlot);
        }
    }
}
