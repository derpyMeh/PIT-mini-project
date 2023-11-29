// Recycled from Jacob Villumsen's P4 3D solo project
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] UnityEngine.AI.NavMeshAgent navMeshAgent;
    [Header("Enemy Modifiers")]
    [SerializeField] float startWaitTime = 2; 
    [SerializeField] float timeToRotate = 2, speedWalk = 6, speedRun = 9, viewRadius = 15, viewAngle = 90;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public Transform[] waypoints;
    int currentWayPointIndex;

    Vector3 playerLastNearbyPos = Vector3.zero;
    Vector3 playerLastSeenPos;

    float waitDelay, waitToRotate;
    bool playerInRange, playerNear, isPatrolling, caughtPlayer;


   
    void Start()
    {
        playerLastSeenPos = Vector3.zero;
        isPatrolling = true;
        caughtPlayer = false;
        playerInRange = false;
        playerNear = false;
        waitDelay = startWaitTime;
        waitToRotate = timeToRotate;

        currentWayPointIndex = 0;                                               // (Re)sets to first waypoint
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[currentWayPointIndex].position); // Sets destination to first waypoint on game launch  
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
            navMeshAgent.SetDestination(playerLastSeenPos); // Sets destination to player location
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
                navMeshAgent.SetDestination(waypoints[currentWayPointIndex].position);
            }
            else
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                {
                    Stop();
                    waitDelay -= Time.deltaTime;
                }
            }
        }
    }

    void Patrolling()
    {
        if (playerNear) // If player is nearby, wait for a bit and then move to player's position
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
            navMeshAgent.SetDestination(waypoints[currentWayPointIndex].position);
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

    void NextPoint()
    {
        currentWayPointIndex = (currentWayPointIndex +1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[currentWayPointIndex].position);
    }

    void Stop()
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
                navMeshAgent.SetDestination(waypoints[currentWayPointIndex].position);
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
            if (Vector3.Distance(transform.position, player.position) > viewRadius) // If player is outside of viewRadius (range), set playerInRange to false.
            {
                playerInRange = false;
            }
            if (playerInRange) // Obtain player's current pos if in range
            {
                playerLastSeenPos = player.transform.position;
            }
        }
    }
}
