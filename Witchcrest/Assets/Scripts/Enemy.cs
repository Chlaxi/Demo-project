﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected int maxHealth;
    protected int health;

    [Header("Damage")]
    [SerializeField] private int damage;
    [SerializeField] private float knockback;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D collider;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private PhysicsMaterial2D deathPhysicsMat;

    private EnemyAI AI;
    private void Start()
    {
        AI = GetComponentInParent<EnemyAI>();
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Damage taken: " + damage);
        health -= damage;

        animator.SetTrigger("Hit");

        if(health <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().Hurt(damage);
            Vector3 force = (collision.gameObject.transform.position - transform.position).normalized;
            force.x *= 10;
            Debug.Log(force);
            collision.rigidbody.AddForce(force * knockback, ForceMode2D.Impulse);
        }
    }

    protected void Die()
    {
        rigidbody.gravityScale = 1;
        AI.Stop();
        rigidbody.constraints = RigidbodyConstraints2D.None;
        rigidbody.sharedMaterial = deathPhysicsMat;
        collider.sharedMaterial = deathPhysicsMat;
        animator.SetBool("IsDead", true);
        Debug.Log("Dead");
        gameObject.layer = 13; //Sets their layer to "DeadEnemy" layer, so they won't collide with player, but can still ragdoll a bit.

        //Eventually disappar?
    }

    public void Kill()
    {
        health = 0;
        Die();
    }
}
