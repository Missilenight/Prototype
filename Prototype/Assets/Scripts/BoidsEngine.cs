using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsEngine : MonoBehaviour
{
    [Header("Simulation Properties")]

    [SerializeField]
    bool IS_LAND_ANIMAL = false;

    [SerializeField, Tooltip("Total Agent Count")]
    int AGENT_COUNT  = 0;

    [SerializeField, Tooltip("Agent Prefab")]
    BoidsAgent AGENT_PREFAB;

    [Header("Agent Properties")]

    [SerializeField, Range(1, 360)]
    float FieldOfView = 180f;

    [SerializeField, Min(0)]
    float Speed = 1.0f;

    [Header("Boids Behaviours")]

    [SerializeField, Min(0)]
    float AlignmentWeight = 1f;

    [SerializeField, Min(0)]
    float CohesionWeight = 1f;

    [SerializeField, Min(0)]
    float SeparationWeight = 1f;

    [SerializeField, Min(0), Tooltip("The minimum distance each agent should keep from the flock mates")]
    float SeparationRadius = 5f;

    [SerializeField, Min(0), Tooltip("The distance of which an agent becomes part of a neighbourhood")]
    float NeighbourRadius = 10f;

    [Header("Extra Behaviours")]

    [SerializeField, Min(0)]
    float SteerWeight = 1f;

    [SerializeField, Min(0)]
    float LeaderWeight = 1f;

    [SerializeField, Min(0)]
    float BoundsWeight = 1f;

    [SerializeField, Min(0)]
    float BoundsDistance = 1f;

    [SerializeField, Min(0)]
    float FearWeight = 1f;

    [SerializeField, Min(0)]
    float FearDistance = 1f;

    [SerializeField, Min(0)]
    float AttackWeight = 1f;

    [SerializeField, Min(0)]
    float AttackDistance = 1f;

    [SerializeField, Min(0)]
    float ObjectAvoidanceWeight = 1f;

    [SerializeField, Min(0)]
    float ObjectAvoidanceDistance = 1f;

    private List<BoidsAgent> Agents = new List<BoidsAgent>();

    private void Start()
    {
        // Get the total bounds of the gameObject
        Vector3 scale = transform.localScale;

        for (int i = 0; i < AGENT_COUNT; i++)
        {
            Vector3 spawnLocation = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;

            if (IS_LAND_ANIMAL)
            {
                // For land animals we will try to spawn the animals as grounded as possible
                RaycastHit hit;
                
                Vector3 chosenPoint = transform.position + new Vector3(Random.Range(-scale.x/2, scale.x/2), 0, Random.Range(-scale.z/2, scale.z/2));

                spawnLocation = chosenPoint;

                if (Physics.Raycast(chosenPoint, -Vector3.up, out hit, scale.y * 2))
                    spawnLocation.y = hit.point.y + AGENT_PREFAB.transform.localScale.y/2;

                float y = Random.Range(0f, 2f * Mathf.PI);

                spawnRotation = Quaternion.Euler(0, y, 0);
            }
            else
            {
                // For air animals they will spawn anywhere within the box
                spawnLocation = transform.position + new Vector3(Random.Range(-scale.x / 2, scale.x / 2), Random.Range(-scale.y / 2, scale.y), Random.Range(-scale.z / 2, scale.z / 2));

                float x = Random.Range(0f, 2f * Mathf.PI);
                float y = Random.Range(0f, 2f * Mathf.PI);
                float z = Random.Range(0f, 2f * Mathf.PI);

                spawnRotation = Quaternion.Euler(x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, z * Mathf.Rad2Deg);
            }

            // Spawn the agent
            BoidsAgent agent = Instantiate<BoidsAgent>(AGENT_PREFAB, spawnLocation, spawnRotation);
            
            Agents.Add(agent);
        }
    }

    // Update every 60FPS
    private void Update()
    {
        foreach (BoidsAgent agent in Agents)
        {
            // Set the bounds, this way we can update it in real time
            agent.SetBounds(transform.position, transform.localScale);

            // Set the base properties
            agent.SetProperties(Speed, FieldOfView);

            // Update everything
            agent.UpdateBehaviour(
                Agents,
                // Boids Behaviours
                AlignmentWeight, CohesionWeight, SeparationWeight, SeparationRadius, NeighbourRadius,
                // Additional Behaviours
                SteerWeight, LeaderWeight, BoundsWeight, BoundsDistance, FearWeight, FearDistance, AttackWeight, AttackDistance, ObjectAvoidanceWeight, ObjectAvoidanceDistance
            );
        }
    }
}
