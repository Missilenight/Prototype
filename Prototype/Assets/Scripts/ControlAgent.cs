using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControlAgent : MonoBehaviour
{
    public float speed = 5f; // Entity's speed
    public float lerpTime = 0.2f; // Lerp transition time

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GameObject target = FindTarget();
        if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;

            // If target tag is "Target", entity moves towards target
            if (target.tag == "Target")
            {
                MoveEntity(direction.normalized);
            }
            // If target tag is "Predator", entity moves away from target
            else if (target.tag == "Predator")
            {
                MoveEntity(-direction.normalized);
            }
        }
    }

    // Find the target in scene
    GameObject FindTarget()
    {
        GameObject[] targets;
        targets = GameObject.FindGameObjectsWithTag("Target");
        if (targets.Length > 0)
        {
            return targets[0]; // Just consider the first target found
        }

        GameObject[] predators;
        predators = GameObject.FindGameObjectsWithTag("Predator");
        if (predators.Length > 0)
        {
            return predators[0]; // Just consider the first predator found
        }
        return null;
    }

    // Move entity towards or away from a target
    void MoveEntity(Vector3 direction)
    {
        Vector3 newVelocity = Vector3.Lerp(rb.velocity, direction * speed, lerpTime);
        rb.velocity = newVelocity;
    }
}