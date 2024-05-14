using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage; // (When caculating damage this is used to check for attackers damage modfiers, effects ect)
    }
}
