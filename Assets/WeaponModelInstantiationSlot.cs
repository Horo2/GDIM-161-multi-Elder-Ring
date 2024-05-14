using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class WeaponModelInstantiationSlot : MonoBehaviour
    {
        public WeaponModelSlot weaponSlot;
        public GameObject currentWeaponModel;

        public void UnloadWeapon()
        {
            if(currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeapon(GameObject weponModel)
        {
            currentWeaponModel = weponModel;
            weponModel.transform.parent = transform;

            weponModel.transform.localPosition = Vector3.zero;
            weponModel.transform.localRotation = Quaternion.identity;
            weponModel.transform.localScale = Vector3.one;
        }
    }
}
