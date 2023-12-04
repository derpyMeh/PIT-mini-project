using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int enemyHealth = 10;
    int currentHealth;
    bool isDead = false;

    [SerializeField] Animator orcWarriorAnimator;

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

                // Stop all currently active animations
                orcWarriorAnimator.SetBool("IsMoving", false);
                orcWarriorAnimator.SetBool("IsAttacking", false);

                // Trigger death animation
                orcWarriorAnimator.SetBool("IsDead", true);

                // Wait for the death animation to end (assuming it takes 3 seconds)
                StartCoroutine(WaitAndDestroy(3f));
            }
        }
    }

    IEnumerator WaitAndDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // Trigger self destruction after waiting
        GetComponent<AIBehavior>().SelfDestruct();
    }
}
