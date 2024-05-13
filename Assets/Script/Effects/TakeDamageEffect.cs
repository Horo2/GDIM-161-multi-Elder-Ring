using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {

        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage;     // If the damage is caused by another characters attack it will be stored here

        [Header("Damage")]
        public float physicalDamage = 0;                    // (In the future will be split into "Standard, "STrike", "Slash" and "Pierce")
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("Final Damage")]
        public int finalDamageDealt = 0;                  // The damage the character takes after all calculations have been made

        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false;                  // If a character's poise is broken, they will be "Stunned" and play a damage animation

        // (TO DO) Bulild Ups
        // build up effect amounts

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("Sound FX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundFX;            // Used on top of regular SFX if there is elemental damage present(Magic/Fire/Lightning/Holy)

        [Header("Direction Damage Taken From")]
        public float angleHitFrom;                          // Used to determine what damage animation to play(Move backwards, to the left, to the right etc)
        public Vector3 contacktPoint;                       // Used to determine where the blood FX instantiate
       
        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            // If the character is dead, no additional damage effects should be processed
            if (character.isDead.Value)
                return;

            // Check for "Invulnerability"

            // Calculate damage
            CalculateDamage(character);

            // Check which directional damage came from
            // Play a damage animation
            // Check for build ups(Poision, Bleed etc)
            // Play Damage sound FX
            // Play Damage VFX (Blood)

            // If character is A.I, check for new target if character causing damage is present
        }

        private void CalculateDamage(CharacterManager character)
        {
            if(!character.IsOwner) 
                return;

            if(characterCausingDamage != null)
            {
                // Check for damage modifires and modify base damage (Physical/Elemental Damage buff)

            }

            // Check character for flat defensed and subtract them from the damage

            // Check character for armor absorptions, and subtract the percentage from the damage

            // Add all damage types together, and apply final damage
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if(finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }

            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

            //Calculate poise damage to determine if the character will be stunned
        }
    }
}
