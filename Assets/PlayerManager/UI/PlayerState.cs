using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("Health")]
    public int healthLevel = 10;
    public int maxHealth;
    public int currentHealth;
    public HealthBar healthBar;

    [Header("Mana")]
    public int manaLevel = 10;
    public int maxMana;
    public int currentMana;
    public ManaBar manaBar;
    public bool hasMana = true;
    public bool Ultimate = true;

    AnimationManager animationManager;

    private void Awake()
    {
        animationManager = GetComponentInChildren<AnimationManager>();
    }
    void Start()
    {
        //Health
        maxHealth = SetMaxHealth_FromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        maxMana = SetMaxMana_FromManathLevel();
        currentMana = maxMana;
        manaBar.SetMaxMana(maxMana);
    }

    private int SetMaxHealth_FromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public void TakeDame(int dame)
    {
        currentHealth -= dame;
        healthBar.SetCurrentHealth(currentHealth);
        animationManager.PlayerTargetAnimation("Dealing", true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animationManager.PlayerTargetAnimation("Death", true);
        }
    }

    private int SetMaxMana_FromManathLevel()
    {
        maxMana = manaLevel * 10;
        return maxMana;
    }

    public void manaSkill(int mana)
    {
        currentMana -= mana;
        manaBar.SetCurrentMana(currentMana);
        if (currentMana <= 0)
        {
            currentMana = 0;
            hasMana = false;
        }else if(currentMana > 0)
        {
            hasMana= true;
        }
    }

    public void recoverMana(int manaRecover)
    {
        currentMana += manaRecover;
        manaBar.SetCurrentMana(currentMana);
        if (currentMana > 0)
        {
            hasMana = true;
            if(currentMana == maxMana)
            {
                Ultimate = true;
                currentMana = maxMana;
                Debug.Log("Ultimate" );
            }
        }
        else if (currentMana <= 0)
        {
            hasMana = false;
            Ultimate = false;
        }
    }
}
