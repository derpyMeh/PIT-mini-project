using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCycler : MonoBehaviour
{
    [SerializeField] Animator orcWarriorAnimator;

    Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        // Check if the object is moving
        if (IsMoving())
        {
            // Set the Run parameter to true
            orcWarriorAnimator.SetBool("IsMoving", true);
        }
        else
        {
            // Set the Run parameter to false
            orcWarriorAnimator.SetBool("IsMoving", false);
        }

        // Check if the object is near the player
        if (IsNearPlayer())
        {
            // Set the Attack1 parameter to true
            orcWarriorAnimator.SetBool("IsAttacking", true);
        }
        else
        {
            // Set the Attack1 parameter to false
            orcWarriorAnimator.SetBool("IsAttacking", false);
        }
    }

    bool IsMoving()
    {
        // Check if the position has changed
        bool moving = transform.position != previousPosition;
        // Update the previous position
        previousPosition = transform.position;
        return moving;
    }

    bool IsNearPlayer()
    {
        // Check if the object is near an object with the tag "Player"
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
