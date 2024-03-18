using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverRide;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;

        public GameObject currentWeaponModel;


        public void UnloadWeapon()
        {
            if(currentWeaponModel != null)
            {
                currentWeaponModel.SetActive(false);
            }
        }

        public void UnloadWeaponAndDestroy()
        {
            if(currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }
        public void LoadWeaponModel(WeaponItem weaponItem)
        {
            UnloadWeaponAndDestroy();
            if(weaponItem == null)
            {
                UnloadWeapon();
                return;
            }
            //Instantiate khởi tạo (sap chép một prefab) thường được dùng để tạo đạn bắn v.v....
            GameObject model = Instantiate(weaponItem.modelPrefab) as GameObject; //as..... trả về kiểu dữ liệu nào đó dc chỉ định
            if(model != null)
            {
                if(parentOverRide != null)
                {
                    model.transform.parent = parentOverRide;
                }else
                {
                    model.transform.parent = transform;
                }

                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }

            currentWeaponModel = model;
        }
    }

}
