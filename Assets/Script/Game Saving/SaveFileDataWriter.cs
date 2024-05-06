using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Rendering;
using System.Linq.Expressions;

namespace Horo
{
    public class SaveFileDataWriter
    {
        public string saveDataDirectoryPath = "";
        public string saveFileName = "";

        // Before we create a new asve file, we must scheck to see if one of this character slot already exists (Max 10 character slots)
        public bool CheckToSeeIfFileExists()
        {
            if(File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
            {
                return true;
            }
            else 
            { 
                return false; 
            }

        }

        // Used to delete character save files
        public void DletedSaveFile()
        {
            File.Delete(Path.Combine(saveDataDirectoryPath , saveFileName));
        }

        // Used to create a save file upon starting a new game
        public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
        {
            // Make a path to save the file
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // Create the directory the file will be written to, if it does not already exist
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Creating SAVE FILE, AT SAVE PATH: " + savePath);

                // Serialize the C# game data object into json
                string dataToStore = JsonUtility.ToJson(characterData, true);

                // Write the file to our system
                using(FileStream stream = new FileStream(savePath,FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex) 
            {
                Debug.LogError("ERROR WHILST TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED" + savePath + "\n" + ex);
            }
        }

        // Used to load a save file upon loading aprevious game
        public CharacterSaveData loadSaveFile()
        {
            CharacterSaveData characterData = null;
            // Make a path to load the file
            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            if(File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // Deserialize the data from json back to Unity
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception ex)
                {
                }
            }
            return characterData;           
        }
    }
}

