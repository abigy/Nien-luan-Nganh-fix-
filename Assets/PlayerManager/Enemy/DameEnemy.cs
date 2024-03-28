using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DameEnemy : MonoBehaviour
{
    public int damage = 25;
    private void OnTriggerEnter(Collider other)
    {
        EnemyState EnemyStates = other.GetComponent<EnemyState>();

        if (EnemyStates != null)
        {
            EnemyStates.TakeDame(damage);
        }
    }
}
