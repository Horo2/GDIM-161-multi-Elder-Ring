using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    [System.Serializable]
    //Since we want to reference this data for every save file, this script is no a monobehaviour and is instead serializable
    public class CharacterSaveData
    {
        [Header("Scene Index")]
        public int sceneIndex = 1;

        [Header("Character Name")]
        public string characterName = "Character";

        [Header("Time Played")]
        public float sceondsPlayed;

        [Header("Wolrd Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Resources")]
        public float currentHealth;
        public float currentStamina;

        [Header("Stats")]
        public int vitality;
        public int endurance;
    }
}
