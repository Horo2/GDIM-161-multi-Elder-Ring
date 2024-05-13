using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

namespace Horo
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Stamina Regenration")]
        [SerializeField] float staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] float staminaRegenerationDelay = 2;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
            
        }

        public int CalcuateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            // Create an equation for how you want your stamina to be calculated
            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        public int CalcuateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            // Create an equation for how you want your stamina to be calculated
            health = vitality * 15;

            return Mathf.RoundToInt(health);
        }

        public virtual void RegencerateStamina()
        {
            // Only owners can edit their network varaibles
            if (!character.IsOwner)
            {
                return;
            }
            // we do not want to regenerate stamina if we are using it
            if (character.characterNetworkManager.isSprinting.Value)
            {
                return;
            }
            if (character.isPerformingAction)
            {
                return;
            }

            staminaRegenerationTimer += Time.deltaTime;
            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                    }
                }
            }
        }

        public virtual void ResetStaminaRgenTimer(float previousStaminaAmount, float currentStaminaAmount) 
        {
            // We only wnat to reset the regeneration if the action used stamina
            // We dont want to reset the regeneration if we are alreday regenerating stamina
            if(currentStaminaAmount < previousStaminaAmount)
            {
                staminaRegenerationTimer = 0;
            }
                       
        }
    }
}
