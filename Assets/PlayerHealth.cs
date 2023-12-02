using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] CombatSystem combatSystem;
    [SerializeField] int playerHealth = 10;
    int playerHP;
    public TextMeshProUGUI HPStat;

    void Awake()
    {
        playerHP = playerHealth;
        HPStat.text = ("HP: " + playerHP);
    }


    public void PlayerTakeDamage(int amount)
    {
        playerHP -= amount;
        HPStat.text = ("HP: " + playerHP);
        if (playerHP <= 0)
        {
            // Game Over
            combatSystem.PlayerAlive = false;
        }
    }
}
