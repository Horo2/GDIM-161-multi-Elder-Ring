using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    [System.Serializable]
    //Since we want to reference this data for every save file, this script is no a monobehaviour and is instead serializable
    public class CharacterSaveData
    {        
        [Header("Character Name")]
        public string characterName;

        [Header("Time Played")]
        public float sceondsPlayed;

        [Header("Wolrd Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;
    }
}
