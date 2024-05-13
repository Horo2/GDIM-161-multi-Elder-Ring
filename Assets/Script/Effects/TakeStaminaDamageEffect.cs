using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage;

        public override void ProcessEffect(CharacterManager character)
        {
            CalculateStaminaDamage(character);
        }

        private void CalculateStaminaDamage(CharacterManager character)
        {
            // Compared the bas estamina damage against other player effects/modifiers
            // Change the value before subtacting/adding it
            // Play sound FX or VFX during effect

            if(character.IsOwner)
            {
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            }
        }
    }
}
