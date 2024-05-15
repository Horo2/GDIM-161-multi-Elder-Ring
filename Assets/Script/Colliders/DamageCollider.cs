using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Collider")]
        protected Collider damagecollider;

        [Header("Damage")]
        public float physicalDamage = 0;                    // (TO DO, In the future will be split into "Standard, "STrike", "Slash" and "Pierce")
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("Contact Point")]
        protected Vector3 contactPoint;

        [Header("Chracters Damaged")]
        protected List<CharacterManager> characterDamaged = new List<CharacterManager>();

        private void OnTriggerEnter(Collider other)
        {
           CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            // If you want to search on both the damageable character colliders & the character controller collider just check for null here and do the following
            //if(damageTarget == null )
            //{
            //    damageTarget = other.GetComponent<CharacterManager>();
            //}

            if(damageTarget != null)
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                // Check if we can damage this target based on firendly fire

                // Check if target is blocking

                // Check if target is Invulnerable

                // Damage
                DamageTarget(damageTarget);
            }
        }

        protected virtual void DamageTarget(CharacterManager damageTarget)
        {
            // We don't want to damage the same target more than once in a single attack
            // So we add them to a list that checks before applying damage

            if(characterDamaged.Contains(damageTarget))
                return;

            characterDamaged.Add(damageTarget);

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contacktPoint = contactPoint;

            damageTarget.characterEffectManager.ProcessInstantEffect(damageEffect);
        }

        public virtual void EnableDamageCollider()
        {
            damagecollider.enabled = true;
        }

        public virtual void DisableDamageCollider()
        {
            damagecollider.enabled = false;
            characterDamaged.Clear(); // We reset the characters that have been hit when we reset the collider, so they may be hit again
        }
    }
}
