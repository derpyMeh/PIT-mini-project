// Recycled from Jacob Villumsen's P4 3D solo project
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehavior : MonoBehaviour
{
    [SerializeField] UnityEngine.AI.NavMeshAgent navMeshAgent;
    [Header("Enemy Modifiers")]
    [SerializeField] float startWaitTime = 3;
    [SerializeField] float timeToRotate = 2, speedWalk = 6, speedRun = 9, viewRadius = 15, viewAngle = 90;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [SerializeField] List<Transform> waypoints;
    int currentWayPointIndex;

    Vector3 playerLastNearbyPos = Vector3.zero;
    Vector3 playerLastSeenPos;

    float waitDelay, waitToRotate;
    bool playerInRange, playerNear, isPatrolling, caughtPlayer;

    [SerializeField] int damageAmount = 1; // Amount of damage the enemy does to the player

    void Awake()
    {
        playerLastSeenPos = Vector3.zero;
        isPatrolling = true;
        caughtPlayer = false;
        playerInRange = false;
        playerNear = false;
        waitDelay = startWaitTime;
        waitToRotate = timeToRotate;

        currentWayPointIndex = 0;                                               // (Re)sets to the first waypoint
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[currentWayPointIndex].transform.position); // Sets the destination to the first waypoint on game launch
    }

    void Update()
    {
        EnvironmentView(); // Checking for line of sight & range
        if (!isPatrolling)
        {
            Chasing();
        }
        else
        {
            Patrolling();
        }
    }

    void Chasing()
    {
        playerNear = false;                 // Set to false, since the player has already been noticed
        playerLastNearbyPos = Vector3.zero; // Resets the variable

        if (!caughtPlayer)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(playerLastSeenPos); // Sets destination to the player's location
        }
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (waitDelay <= 0 && !caughtPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                isPatrolling = true;
                playerNear = false;
                Move(speedWalk);
                waitToRotate = timeToRotate;
                waitDelay = startWaitTime;
                navMeshAgent.SetDestination(waypoints[currentWayPointIndex].transform.position);
            }
            else
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                {
                    Stop();
                    waitDelay -= Time.deltaTime;
                }
                else
                {
                    // Damage the player and self-destruct when in range
                    DamagePlayer();
                }
            }
        }
    }

    void Patrolling()
    {
        if (playerNear) // If the player is nearby, wait for a bit and then move to the player's position
        {

            if (waitToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastNearbyPos);
            }
            else
            {

                Stop();
                waitToRotate -= Time.deltaTime;
            }
        }
        else
        {
            playerNear = false;
            playerLastNearbyPos = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[currentWayPointIndex].transform.position);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (waitDelay <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    waitDelay = startWaitTime;
                }
                else
                {
                    Stop();
                    waitDelay -= Time.deltaTime;
                }
            }
        }
    }

    void DamagePlayer()
    {
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.PlayerTakeDamage(damageAmount); // Damage the player using the PlayerHealth reference
            SelfDestruct(); // Self-destruct after damaging the player
        }
        Debug.Log($"Player Damaged: Health: {playerHealth.PlayerHP}");
    }

    public void SelfDestruct()
    {
        Destroy(gameObject); // Self-destruct the enemy
    }

    void NextPoint()
    {
        currentWayPointIndex = (currentWayPointIndex + 1) % waypoints.Count;
        navMeshAgent.SetDestination(waypoints[currentWayPointIndex].transform.position);
    }

    public void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void CaughtPlayer()
    {
        caughtPlayer = true;
        Debug.Log("Calling DamagePlayer function now.");
        DamagePlayer();
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (waitDelay <= 0)
            {
                playerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[currentWayPointIndex].transform.position);
                waitDelay = startWaitTime;
                waitToRotate = timeToRotate;
            }
            else
            {
                Stop();
                waitDelay -= Time.deltaTime;
            }
        }
    }

    void EnvironmentView()
    {
        Collider[] isPlayerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask); // makes an OverLapSphere to detect the player while inside viewRadius area.
        for (int i = 0; i < isPlayerInRange.Length; i++)
        {
            Transform player = isPlayerInRange[i].transform;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2) // If player is within view range and angle, and not behind an obstacle, set playerInRange to true.
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    playerInRange = true;
                    isPatrolling = false;
                }
                else
                {
                    playerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > viewRadius) // If the player is outside of viewRadius (range), set playerInRange to false.
            {
                playerInRange = false;
            }
            if (playerInRange) // Obtain the player's current position if in range
            {
                playerLastSeenPos = player.transform.position;
            }
        }
    }
}