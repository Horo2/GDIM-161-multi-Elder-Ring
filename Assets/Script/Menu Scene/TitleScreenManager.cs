using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


namespace Horo
{
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Menu")]
        [SerializeField] GameObject titleScreenMainMenu;
        [SerializeField] GameObject titleScreenLoadMenu;

        [Header("Buttons")]
        [SerializeField] Button LoadMenuReturnButton;
        [SerializeField] Button MainMenuLoadGameButton;
        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.instance.CreateNewGame();
            StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
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
    }
}

