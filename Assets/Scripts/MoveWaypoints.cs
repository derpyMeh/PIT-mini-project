using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWaypoints : MonoBehaviour
{
    [SerializeField] private Waypoint waypoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float minDistance = 0.1f;

    private Transform currentWayPoint;
    private Quaternion roationGoal;
    private Vector3 directionWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        currentWayPoint = waypoints.GetWayPoints(currentWayPoint);
        transform.position = currentWayPoint.position;
        currentWayPoint = waypoints.GetWayPoints(currentWayPoint);
        transform.LookAt(currentWayPoint);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentWayPoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentWayPoint.position) < minDistance)
        {
            currentWayPoint = waypoints.GetWayPoints(currentWayPoint);
        }
        LookAtWaypoint();
    }

    private void LookAtWaypoint()
    {
        directionWaypoint = (currentWayPoint.position - transform.position).normalized;
        roationGoal = Quaternion.LookRotation(directionWaypoint);
        transform.rotation = Quaternion.Slerp(transform.rotation, roationGoal, rotationSpeed * Time.deltaTime);

    }
}
