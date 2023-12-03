using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int enemyHealth = 10;
    int currentHealth;
    bool isDead = false;

    void Awake()
    {
        currentHealth = enemyHealth;
    }

    public void TakeDamage(int amount)
    {
        if (!isDead) // Check if the enemy is not already dead
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
                GetComponent<AIBehavior>().SelfDestruct(); // Trigger death behavior in AIBehavior script
            }
        }
    }
}
