using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class CharacterEffectManager : MonoBehaviour
    {
        // Process Instant effects (take damage, heal)

        // Process timed effects(Poison, buld ups)

        // Process static effects(adding/removing buffs from talismans ECT)

        CharacterManager character;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            // Take in an effect
            // Process it
            effect.ProcessEffect(character);
        }
    }
}
