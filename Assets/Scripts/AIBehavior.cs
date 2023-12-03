using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehavior : MonoBehaviour
{
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Animator orcWarriorAnimator;
    [SerializeField] Transform destinationObject;
    [SerializeField] float startWaitTime = 2, timeToRotate = 2, speedWalk = 6, speedRun = 9, viewRadius = 15, viewAngle = 90;
    [SerializeField] List<Transform> waypoints;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] int damageAmount = 1;

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

        currentWayPointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[currentWayPointIndex].transform.position);
    }

    void Update()
    {
        EnvironmentView();
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
        playerNear = false;
        playerLastNearbyPos = Vector3.zero;

        if (!caughtPlayer)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(playerLastSeenPos);
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
                    DamagePlayer();
                }
            }
        }
    }

    void Patrolling()
    {
        if (playerNear)
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
            playerHealth.PlayerTakeDamage(damageAmount);
            SelfDestruct();
        }
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }

    void NextPoint()
    {
        currentWayPointIndex = (currentWayPointIndex + 1) % waypoints.Count;
        navMeshAgent.SetDestination(waypoints[currentWayPointIndex].transform.position);
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
        orcWarriorAnimator.SetTrigger("IdleTrigger");
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
        orcWarriorAnimator.SetTrigger("RunTrigger");
    }

    void CaughtPlayer()
    {
        caughtPlayer = true;
        orcWarriorAnimator.SetTrigger("AttackTrigger");
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
        Collider[] isPlayerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for (int i = 0; i < isPlayerInRange.Length; i++)
        {
            Transform player = isPlayerInRange[i].transform;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
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
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                playerInRange = false;
            }
            if (playerInRange)
            {
                playerLastSeenPos = player.transform.position;
            }
        }
    }
}
