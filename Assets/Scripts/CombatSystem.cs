using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Add Prefabs Here")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] List<GameObject> enemySpawnPoints;
    [Header("Respawn Parameters")]
    [SerializeField] float minimumWaveTime = 2;
    [SerializeField] float maximumWaveTime = 5;
    [SerializeField] int spawnMultiplier = 1;

    private float timeUntilSpawn;
    private int waveCount;

    // kill counter basically
    private static int playerScore;
    private static int baseHP;
    private static bool playerAlive;

    private bool combatInitiated;

    void Start()
    {
        combatInitiated = false;
        spawnMultiplier = 1;
        waveCount = 0;
        playerScore = 0;
        baseHP = 100;
        playerAlive = true;
        timeUntilSpawn = 0;
        StartCombat();
    }

    void Update()
    {
        if (!playerAlive)
        {
            // Game Over
            combatInitiated = false;
        }
        timeUntilSpawn -= Time.deltaTime;
        if (timeUntilSpawn <= 0 && combatInitiated)
        {
            SpawnEnemies();
        }
    }

    private void StartCombat()
    {
        Debug.Log("Combat Initiated...");
        combatInitiated = true;
    }

    private void SetTimeUntilSpawn()
    {
        timeUntilSpawn = Random.Range(minimumWaveTime, maximumWaveTime);
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < spawnMultiplier; i++)
            {
                for (int j = 0; j < enemySpawnPoints.Count; j++)
                {
                    Instantiate(enemyPrefab, enemySpawnPoints[j].transform.position, Quaternion.identity);
                }
            } 
            SetTimeUntilSpawn();
            waveCount += 1;
    }

    public void InitiateCombat()
    {
        StartCombat();
    }

    public bool GetCombatState
    {
        get
        {
            return combatInitiated;
        }
    }

    public int PlayerScore
    {
        get
        {
            return playerScore;
        }
        set
        {
            playerScore += value;
        }
    }

    public int BaseHP
    {
        get
        {
            return baseHP;
        }
        set
        {
            baseHP += value;
        }
    }

    public bool PlayerAlive
    {
        get
        {
            return playerAlive;
        }
        set
        {
            playerAlive = value;
        }
    }
}
