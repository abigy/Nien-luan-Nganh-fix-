using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DameEnemy : MonoBehaviour
{
    public int damage = 25;
    private void OnTriggerEnter(Collider other)
    {
        EnemyState playerStates = other.GetComponent<EnemyState>();

        if (playerStates != null)
        {
            playerStates.TakeDame(damage);
        }
    }
}
