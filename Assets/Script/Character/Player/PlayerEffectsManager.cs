using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class PlayerEffectsManager : CharacterEffectManager
    {
        [Header("Debug Delete Later")]
        [SerializeField] InstantCharacterEffect effectToTest;
        [SerializeField] bool processEffect = false;

        private void Update()
        {
            if(processEffect)
            {
                processEffect = false;
                // When we instantiate it, the original is not effected
                InstantCharacterEffect effect = Instantiate(effectToTest);

                //When we dont instaniate it, the original is changed(you do not want this in most case)
                //effectToTest.staminaDamage = 55;

                ProcessInstantEffect(effect);
            }
        }
    }
}
