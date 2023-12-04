using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] CombatSystem combatSystem;
    [SerializeField] int playerHealth = 10;
    int playerHP;

    void Awake()
    {
        playerHP = playerHealth;
    }


    public void PlayerTakeDamage(int amount)
    {
        playerHP -= amount;
        if (playerHP <= 0)
        {
            // Game Over
            combatSystem.PlayerAlive = false;
            Debug.Log("Player Died");
        }
    }


    public int PlayerHP
    {
        get
        {
            return playerHP;
        }
        set
        {
            playerHP = value;
        }
    }
}
