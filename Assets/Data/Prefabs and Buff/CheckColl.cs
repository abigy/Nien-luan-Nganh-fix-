using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckColl : MonoBehaviour
{
    public GameObject hitColl;

    private bool collided;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Slash" || collision.gameObject.tag != "Enemy" || collision.gameObject.tag != "Player" && !collided)
        {
            collided = true;
            var inpact = Instantiate(hitColl, collision.contacts[0].point, Quaternion.identity) as GameObject;
            Destroy(inpact, 0.2f);
            Destroy(gameObject);
        }
    }
}
