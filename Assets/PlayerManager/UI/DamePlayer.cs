using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamePlayer : MonoBehaviour
{
    public int damage = 25;
    private void OnTriggerEnter(Collider other)
    {
        PlayerState playerStates = other.GetComponent<PlayerState>();

        if (playerStates != null)
        {
            playerStates.TakeDame(damage);
        }
    }
}
