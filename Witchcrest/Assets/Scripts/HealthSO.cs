using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

[CreateAssetMenu(fileName ="Health")]
public class HealthSO : ScriptableObject
{
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;

    public int Hurt(int damage)
    {
        currentHealth -= damage;
        CheckHealth();
        return damage;
    } 

    public int Heal(int healing)
    {
        if (maxHealth - currentHealth < healing)
            Debug.LogFormat("Overheal! {1} hp to restore, tried healing {2}", maxHealth - currentHealth, healing);
        
        currentHealth += healing;
        CheckHealth();

        return healing;
    }

    public void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            Debug.LogWarning("YOU DIED");
        }

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public bool IsDead()
    {
        if (currentHealth > 0)
            return false;

        return true;
    }
}
