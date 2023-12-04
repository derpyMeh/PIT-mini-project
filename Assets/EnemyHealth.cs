using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int enemyHealth = 10;
    [SerializeField] int gunDamage = 5;
    NavMeshAgent navAgent;
    int currentHealth;
    bool isDead = false;

    [SerializeField] Animator orcWarriorAnimator;

    void Awake()
    {
        currentHealth = enemyHealth;
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"TakeDamage function triggered. Enemy took {amount} damage.");
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
                Debug.Log("Enemy Died. Starting death animation");
                // Wait for the death animation to end (assuming it takes 3 seconds)
                StartCoroutine(WaitAndDestroy(3f));
            }
        }
    }

    IEnumerator WaitAndDestroy(float waitTime)
    {
        Destroy(navAgent);
        yield return new WaitForSeconds(waitTime);

        // Trigger self destruction after waiting
        GetComponent<AIBehavior>().SelfDestruct();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with enemy detected.");
        if(collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Bullet collision confirmed. Now dealing damage to enemy...");
            TakeDamage(gunDamage);
        }
    }
}
