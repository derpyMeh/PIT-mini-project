using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int enemyHealth = 10;
    int enemyHP;

    void Awake()
    {
        enemyHP = enemyHealth;
    }



    public void TakeDamage(int amount)
    {
        enemyHP -= amount;
        if (enemyHP <= 0)
        {
            Destroy(this);
        }
    }
}
