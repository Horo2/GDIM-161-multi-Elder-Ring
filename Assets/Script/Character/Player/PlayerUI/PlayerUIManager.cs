using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Horo
{
    public class PlayerUIManager : MonoBehaviour
    {

        public static PlayerUIManager instance;

        [Header("Network Join")]
        [SerializeField] private bool startGameAsClient;

        [HideInInspector] public PlayerUIHudManager playerUIHudManager;

        private void Awake()
        {
            if (instance == null) { instance = this; }
            else { Destroy(gameObject); }

            playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();

        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if(startGameAsClient)
            {
                startGameAsClient = false;
                // We must first shut down, because we have started as a host during the title screen
               NetworkManager.Singleton.Shutdown();
                // We then restart, as a client
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}

