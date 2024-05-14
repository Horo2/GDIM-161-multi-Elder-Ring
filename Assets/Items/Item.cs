using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class Item : ScriptableObject
    {
        [Header("Item information")]
        public string itemName;
        public Sprite itemIcon;
        [TextArea] public string itemDescription;
        public int itemID;
        
    }
}
