using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WeaponSlotManager : MonoBehaviour
    {
        WeaponHolderSlot leftHandSlot;
        WeaponHolderSlot rightHandSlot;

        DameCollider LeftHandCollider;
        DameCollider RightHandCollider;

        private void Awake()
        {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); 

            foreach(WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {
                if (weaponSlot.isLeftHandSlot)
                {
                    leftHandSlot = weaponSlot;
                }
                if(weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.LoadWeaponModel(weaponItem);
                LoadLeftWeaponDamageCollider();
            }
            else
            {
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();
            }
        }


        #region Handle Weapon Collider
        public void LoadLeftWeaponDamageCollider()
        {
            LeftHandCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DameCollider>();
        }
        public void LoadRightWeaponDamageCollider()
        {
            RightHandCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DameCollider>();
        }

        public void OnLeftWeaponDamgeCollider()
        {
            LeftHandCollider.EnableDamageCollider();
        }

        public void OnRightWeaponDamgeCollider()
        {
            RightHandCollider.EnableDamageCollider();
        }

        public void CloseLeftWeaponDamgeCollider()
        {
            LeftHandCollider.DisableDamageCollider();
        }

        public void CloseRightWeaponDamgeCollider()
        {
            RightHandCollider.DisableDamageCollider();
        }

        #endregion
    }

}
