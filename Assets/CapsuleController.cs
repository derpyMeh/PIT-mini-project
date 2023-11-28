using UnityEngine;
using UnityEngine.AI;

public class CapsuleController : MonoBehaviour
{
    public Transform destination;  // Set this in the inspector to the destination point

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (destination == null)
        {
            Debug.LogError("Destination not set for CapsuleController script on " + gameObject.name);
        }
        else
        {
            MoveToDestination();
        }
    }

    void MoveToDestination()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(destination.position);
        }
    }
}
