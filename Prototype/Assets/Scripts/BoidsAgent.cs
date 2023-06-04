using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoidsAgent : MonoBehaviour
{
    public bool IS_LAND_ANIMAL = false;

    [SerializeField]
    BoidsBehaviour ATTACK_BEHAVIOUR;

    [SerializeField]
    BoidsBehaviour FEAR_BEHAVIOUR;

    public bool isLeader = false;
    public bool isAttack = false;
    public bool isFeared = false;

    public int RAY_COUNT = 4;
    public float MAX_RAY_ANGLE = 15;

    public List<GameObject> Predators;
    public List<GameObject> Targets;

    public GameObject CurrentTarget;
    public GameObject CurrentPredator;

    private float FieldOfView = 180;
    private float Speed = 10;

    private Vector3 BoundsPosition;
    private Vector3 BoundsSize;

    private Rigidbody rb;

    public Vector3 Velocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateBehaviour(
         // Agents
         List<BoidsAgent> agents,

         // Boids Properties
         float alignmentWeight, float cohesionWeight, float separationWeight, float separationRadius, float nieghbourRadius,

         // Additional Behaviours
         float steerWeight, float leaderWeight, float boundsWeight, float boundsDistance, float fearWeight, float fearDistance, float attackWeight, float attackDistance, float objectAvoidanceWeight, float objectAvoidanceDistance
         )
    {
        if (rb == null)
            return;

        // Getters
        List<BoidsAgent> neighbours = GetNeighbours(FieldOfView, nieghbourRadius, agents);
        Targets = GetTargets(FieldOfView, attackDistance);
        Predators = GetPredators(FieldOfView, fearDistance);
        BoidsAgent followLeader = GetLeader(neighbours);

        // Calculate State changing Behaviours
        Vector3 fear = CalculateFear(Predators, fearDistance) * fearWeight;
        Vector3 attack = CalculateAttack(Targets, attackDistance) * attackWeight;

        // Calculate Boids
        Vector3 cohesion = CalculateCohesion(neighbours) * (isFeared ? FEAR_BEHAVIOUR.CohesionWeight : isAttack ? ATTACK_BEHAVIOUR.CohesionWeight : cohesionWeight);
        Vector3 alignment = CalculateAlignment(neighbours) * (isFeared ? FEAR_BEHAVIOUR.AlignmentWeight : isAttack ? ATTACK_BEHAVIOUR.AlignmentWeight : alignmentWeight);
        Vector3 separation = CalculateSeparation(neighbours, separationRadius, Speed) * (isFeared ? FEAR_BEHAVIOUR.SeparationWeight : isAttack ? ATTACK_BEHAVIOUR.SeparationWeight : separationWeight);

        // Calculate Boundaries
        Vector3 boundary = CalculateBounds(boundsDistance) * boundsWeight;

        // Additional Behaviours
        Vector3 leader = CalculateLeader(followLeader) * (isFeared ? FEAR_BEHAVIOUR.LeaderWeight : isAttack ? ATTACK_BEHAVIOUR.LeaderWeight : leaderWeight);

        // Object Avoidance
        Vector3 avoidance = CalculateObjectAvoidance(objectAvoidanceDistance) * objectAvoidanceWeight;

        // Calculate the final Vector
        Vector3 direction = cohesion + alignment + separation + boundary + leader + fear + attack + avoidance;

        Velocity = Vector3.Lerp(Velocity, transform.forward + direction, (isFeared ? FEAR_BEHAVIOUR.SteerWeight : isAttack ? ATTACK_BEHAVIOUR.SteerWeight : steerWeight));

        if (CurrentTarget != null)
            Velocity = Vector3.Lerp(Velocity, (CurrentTarget.transform.position - transform.position), (isFeared ? FEAR_BEHAVIOUR.SteerWeight : isAttack ? ATTACK_BEHAVIOUR.SteerWeight : steerWeight));

        Velocity.Normalize();

        if (IS_LAND_ANIMAL)
            CalculateLandVelocity();

        transform.forward = Velocity;

        if (isFeared)
            rb.velocity = Velocity * FEAR_BEHAVIOUR.Speed;
        else if (isAttack)
            rb.velocity = Velocity * ATTACK_BEHAVIOUR.Speed;
        else
            rb.velocity = Velocity * Speed;
    }

    public void SetBounds(Vector3 position, Vector3 scale)
    {
        // Set the bounds
        BoundsPosition = position;
        BoundsSize = scale;
    }

    public void SetProperties(float speed, float fieldOfView)
    {
        // Set agent properties
        Speed = speed;
        FieldOfView = fieldOfView;
    }

    public List<BoidsAgent> GetNeighbours(float fieldOfView, float neighboursRadius, List<BoidsAgent> agents)
    {
        List<BoidsAgent> neighbours = new List<BoidsAgent>();

        foreach(BoidsAgent agent in agents)
        {
            if (agent == this) continue;

            RaycastHit hit;
            Vector3 direction = agent.transform.position - transform.position;
            float distance = Vector3.Distance(transform.position, agent.transform.position);
            float angle = Vector3.Angle(transform.forward, direction);

            if (distance < neighboursRadius && angle <= fieldOfView * .5f)
                if (!Physics.Raycast(transform.position, direction, out hit, neighboursRadius, 3))
                    neighbours.Add(agent);
        }

        return neighbours;
    }

    public BoidsAgent GetLeader(List<BoidsAgent> neighbours)
    {
        BoidsAgent leader = null;

        foreach (BoidsAgent agent in neighbours)
        {
            if (agent.isLeader)
            {
                if (leader == null)
                    leader = agent;
                else
                {
                    // Find the closest leader
                    float distanceToLeader = Vector3.Distance(transform.position, leader.transform.position);
                    float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

                    if (distanceToAgent < distanceToLeader)
                        leader = agent;
                }
            }
        }

        if (leader == null)
            isLeader = true;
        else
            isLeader = false;

        return leader;
    }

    public List<GameObject> GetTargets(float fieldOfView, float attackDistance)
    {
        List<GameObject> targets = new List<GameObject>();

        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Target"))
        {
            RaycastHit hit;
            Vector3 direction = target.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);

            if (angle <= fieldOfView * .5f && Physics.Raycast(transform.position, direction, out hit, attackDistance))
            {
                if (hit.collider.gameObject == target)
                    targets.Add(target);
            }    
        }

        return targets;
    }

    public List<GameObject> GetPredators(float fieldOfView, float fearDistance)
    {
        List<GameObject> predators = new List<GameObject>();

        foreach (GameObject predator in GameObject.FindGameObjectsWithTag("Predator"))
        {
            RaycastHit hit;
            Vector3 direction = predator.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);

            if (angle <= fieldOfView * .5f && Physics.Raycast(transform.position, direction, out hit, fearDistance))
                if (hit.collider.gameObject == predator)
                    predators.Add(predator);
        }

        return predators;
    }

    // Behaviours 
    public Vector3 CalculateCohesion(List<BoidsAgent> neighbours)
    {
        Vector3 cohesion = Vector3.zero;

        if (neighbours.Count <= 0)
            return cohesion;

        foreach (BoidsAgent agent in neighbours)
            cohesion += agent.transform.position;

        cohesion /= neighbours.Count;

        cohesion = (cohesion - transform.position).normalized;

        return cohesion;
    }

    public Vector3 CalculateAlignment(List<BoidsAgent> neighbours)
    {
        Vector3 alignment = Vector3.zero;

        if (neighbours.Count <= 0)
            return alignment;

        foreach (BoidsAgent agent in neighbours)
            alignment += agent.transform.forward;

        alignment /= neighbours.Count;

        alignment = alignment.normalized;

        return alignment;
    }

    public Vector3 CalculateSeparation(List<BoidsAgent> neighbours, float separationRadius, float speed)
    {
        Vector3 separation = Vector3.zero;

        if (neighbours.Count <= 0)
            return separation;

        foreach (BoidsAgent agent in neighbours)
        {
            float distance = Vector3.Distance(agent.transform.position, transform.position);

            if (distance > 0 && distance < separationRadius)
            {
                separation = separation + Vector3.Normalize(transform.position - agent.transform.position) / Mathf.Pow(distance, 2) / Time.deltaTime;
            }
        }

        return separation;
    }

    public Vector3 CalculateBounds(float boundsDistance)
    {
        Vector3 boundAvoidance = Vector3.zero;
        Vector3 predictPosition = transform.position + (transform.forward * boundsDistance);

        Bounds bounds = new Bounds(BoundsPosition, BoundsSize);

        if (!bounds.Contains(predictPosition))
            boundAvoidance = (BoundsPosition - transform.position).normalized;  

        return boundAvoidance;
    }

    public Vector3 CalculateLeader(BoidsAgent leader)
    {
        Vector3 direction = Vector3.zero;

        if (isLeader)
            return transform.forward;

        direction = leader.transform.forward.normalized;

        return direction;
    }

    public void CalculateLandVelocity()
    {
        RaycastHit hit;
        bool isGrounded = Physics.Raycast(transform.position, -Vector3.up, out hit, 0.5f);

        if (isGrounded)
        {
            Vector3 velocity = Vector3.ProjectOnPlane(Velocity, hit.normal);

            if (velocity.magnitude > .01f)
                Velocity = velocity;
            else
                Velocity = new Vector3(velocity.x, 0, velocity.z);
        }
        else
            Velocity = new Vector3(Velocity.x, -9 * rb.mass * Time.deltaTime, Velocity.z);
    }

    public Vector3 CalculateFear(List<GameObject> predators, float fearDistance)
    {
        Vector3 fear = Vector3.zero;

        if (predators.Count <= 0)
        {
            CurrentPredator = null;
            isFeared = false;
            return fear;
        }

        // Check if predator is finally out of distance
        if (isFeared && CurrentPredator != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, CurrentPredator.transform.position);

            if (CurrentPredator.tag != "Predator" || distanceToTarget > fearDistance)
            {
                isFeared = false;
                CurrentPredator = null;
            }
        } else if (CurrentPredator == null)
        {
            isFeared = false;
        }

        GameObject fearing = null;

        foreach (GameObject predator in predators)
        {
            if (predator == CurrentPredator)
                continue;

            float distanceToTarget = Vector3.Distance(transform.position, predator.transform.position);

            if (distanceToTarget <= fearDistance)
            {
                if (CurrentTarget != null)
                {
                    if (distanceToTarget < Vector3.Distance(transform.position, CurrentTarget.transform.position))
                        fearing = predator;
                }
                else
                {
                    fearing = predator;
                }
            }
        }

        if (fearing != null)
        {
            // Go to the opposite direction of the target
            fear = -(fearing.transform.position - transform.position).normalized;
            CurrentPredator = fearing;
            isFeared = true;
        }

        return fear;
    }

    public Vector3 CalculateAttack(List<GameObject> targets, float attackDistance)
    {
        Vector3 attack = Vector3.zero;

        if (isFeared || targets.Count <= 0)
        {
            isAttack = false;
            CurrentTarget = null;

            return attack;
        }

        // Check if target is out of distance
        
        if (isAttack && CurrentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, CurrentTarget.transform.position);

            if (CurrentTarget.tag != "Target" || distanceToTarget > attackDistance * 2)
            {
                CurrentTarget = null;
                isAttack = false;
            }
        }

        GameObject attacking = null;

        foreach (GameObject target in targets)
        {
            if (target == CurrentTarget)
                continue;

            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= attackDistance)
            {
                if (CurrentTarget != null)
                {
                    if (distanceToTarget < Vector3.Distance(transform.position, CurrentTarget.transform.position))
                        attacking = target;
                }
                else
                {
                    attacking = target;
                }
            }
        }

        if (attacking != null)
        {
            attack = (attacking.transform.position - transform.position).normalized;
            CurrentTarget = attacking;
            isAttack = true;
        }

        return attack;
    }

    public Vector3 CalculateObjectAvoidance(float avoidanceDistance)
    {   
        Vector3 avoidance = Vector3.zero;

        int rayCountPerDimension = Mathf.RoundToInt(Mathf.Sqrt(RAY_COUNT));

        for (int i = 0; i < rayCountPerDimension; i++)
        {
            for (int j = 0; j < rayCountPerDimension; j++)
            {
                // Calculate the angle for the current ray in the x and y dimensions
                float angleX = (i / (float)(rayCountPerDimension - 1)) * MAX_RAY_ANGLE - MAX_RAY_ANGLE / 2f;
                float angleY = (j / (float)(rayCountPerDimension - 1)) * MAX_RAY_ANGLE - MAX_RAY_ANGLE / 2f;

                // Rotate the ray direction by the calculated angles
                Quaternion rotation = Quaternion.Euler(angleX, angleY, 0);
                Vector3 rayDirection = rotation * transform.forward;

                // Debug.DrawRay(transform.position, rayDirection * avoidanceDistance, Color.green);

                RaycastHit hit;
                if (Physics.Raycast(transform.position, rayDirection, out hit, avoidanceDistance))
                    if (hit.collider.gameObject.tag != "Boid")
                        avoidance += Vector3.Reflect(rayDirection, hit.normal);
            }
        }

        if (avoidance != Vector3.zero)
            avoidance.Normalize();

        return avoidance;
    }
}
