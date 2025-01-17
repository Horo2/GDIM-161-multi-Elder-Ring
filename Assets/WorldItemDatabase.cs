using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Horo
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase instance;

        public WeaponItem unarmedWeapon;

        [Header("Weapons")]
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

        [Header("Items")]
        // A list of every item we have in the game
        private List<Item> items = new List<Item>();


        private void Awake()
        {
            if(instance == null )
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // Add all of our weapons to the list of items
            foreach(var weapon in weapons)
            {
                items.Add(weapon);
            }

            // Assign all of our item a unique item ID
            for(int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }

        public WeaponItem GetWeaponByID(int ID)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }
    }
}
