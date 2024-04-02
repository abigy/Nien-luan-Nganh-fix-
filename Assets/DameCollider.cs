using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DameCollider : MonoBehaviour
{
    Collider damageCollider;
    PlayerState playerState;
    public int currentHitDamage = 25;

    private void Awake()
    {
        playerState = GameObject.Find("Player").GetComponent<PlayerState>();
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerState playerStates = collision.GetComponent<PlayerState>();

            if (playerStates != null)
            {
                playerStates.TakeDame(currentHitDamage);
            }
        }

        if(collision.tag == "Enemy")
        {
            EnemyState enemyState = collision.GetComponent<EnemyState>();

            if (enemyState != null)
            {
                playerState.recoverMana(15);
                enemyState.TakeDame(currentHitDamage);
            }
        }
    }
}
