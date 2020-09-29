using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && collision.GetType() == typeof(CircleCollider2D))
        {
            if(UseItem(collision.gameObject.GetComponent<PlayerController>()))
                Destroy(gameObject);
        }
    }

    public abstract bool UseItem(PlayerController player);
}
