using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Horo
{
    public class WorldSaveGameManager : MonoBehaviour
    {


        public static WorldSaveGameManager instance;


 
        [SerializeField] private int worldSceneIndex = 1;



        private void Awake()
        {
            //There acan only be one instance of this script at one time, if another exists, destroy it
            if (instance == null) { instance = this; }
            else { Destroy(gameObject); }
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator LoadNewGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            yield return null;
        }

        public int GetWoroldSceneIndex()
        {
            return worldSceneIndex;
        }


    }
}
   