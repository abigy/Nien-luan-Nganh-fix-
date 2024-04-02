using SG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    AnimationManager animationManager;
    InputManager inputManager;

    public string lastAttack;

    PlayerManager playerManager;
    private void Awake()
    {
        animationManager = GetComponentInChildren<AnimationManager>();
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent<PlayerManager>();
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if(inputManager.comboFlag)
        {
            animationManager.animator.SetBool("canDoCombo", false);

            if (lastAttack == weapon.Light_Attack_1)
            {
                StartCoroutine(HandleLightLastAttack());
                animationManager.PlayerTargetAnimation(weapon.Light_Attack_2, true);
            }
        }
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        animationManager.PlayerTargetAnimation(weapon.Light_Attack_1, true);

        lastAttack = weapon.Light_Attack_1;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        animationManager.PlayerTargetAnimation(weapon.Light_Attack_1, true);
        lastAttack = weapon.Light_Attack_1;
    }

    private IEnumerator HandleLightLastAttack()
    {
        yield return new WaitForSeconds(2.0f);
    }
}






