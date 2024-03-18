using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager WeaponSlotManager;

        public WeaponItem LeftWeapon;
        public WeaponItem RightWeapon;

        private void Awake()
        {
            WeaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start()
        {
            WeaponSlotManager.LoadWeaponOnSlot(RightWeapon, false);
            WeaponSlotManager.LoadWeaponOnSlot(LeftWeapon, true);
        }
    }

}
