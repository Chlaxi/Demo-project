using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldHazard : MonoBehaviour
{
    [Range(0,10), Tooltip("How much damage does the hazard deal? 0 means instant death")]
    [SerializeField] int damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (damage == 0)
                enemy.Kill();
            else
                enemy.TakeDamage(damage);
            
        }
        if (collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if(damage==0)
                player.Hurt(player.health.GetMaxHealth());
            else
                player.Hurt(damage);
        
            //Teleport to last checkpoint from game manager
        }
    }
}
