using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public int healthLevel = 10;
    public int maxHealth;
    public int currentHealth;
    public HealthBar healthBar;

    AnimationManager animationManager;

    private void Awake()
    {
        animationManager = GetComponentInChildren<AnimationManager>();
    }
    void Start()
    {
        maxHealth = SetMaxHealth_FromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
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
}
