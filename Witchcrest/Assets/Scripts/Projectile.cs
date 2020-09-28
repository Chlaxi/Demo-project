using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileInfo info;
    private float lifeTime;
    private Rigidbody2D rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();      
        lifeTime = info.lifeTime;
        if (!info.useGravity)
        {
            rb.gravityScale = 0;
        }
        Shoot();
    }

    public void Shoot()
    {
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target.z = 0;
        rb.AddForce((target - transform.position).normalized * info.speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {

            enemy.TakeDamage(info.damage);
            Destroy(gameObject);
            //Add to pool
        }
        
        if (info.canBounce)
        {
            //Reduce bounce
            return;
        }
        else
        {
           
            Destroy(gameObject);
            //Add to pool
        }
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
        {
            Destroy(gameObject);
            //Add to pool?
        }
    }

    private void OnDestroy()
    {
        //clean?
    }
}
