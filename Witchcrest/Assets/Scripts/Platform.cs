using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField]
    Collider2D collider;
    [SerializeField]
    SpriteRenderer spr;

    bool PlayerCanPass = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered" + collision.tag);
        if (collision.tag == "Player")
        {
            
            //The player is beneath. Allow to pass through
            if (collision.transform.position.y< transform.position.y)
            {
                PlayerCanPass = true;
                Physics2D.IgnoreCollision(collision, collider, true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //The player is beneath. Allow to pass through
            if (collision.transform.position.y < transform.position.y)
            {
                PlayerCanPass = true;
                Physics2D.IgnoreCollision(collision, collider, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerCanPass = false;
            Physics2D.IgnoreCollision(collision, collider, false);

        }
    }

    private void Update()
    {
        if (spr != null)
        {
            if (PlayerCanPass)
            {
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0.8f);
            }
            else
            {
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
            }
        }
    }
}
