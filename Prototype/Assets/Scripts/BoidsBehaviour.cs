using UnityEngine;

[CreateAssetMenu(fileName = "Boids Behaviour", menuName = "Boids/Behaviour", order = 1)]
public class BoidsBehaviour : ScriptableObject
{
    // DRIVE
    [Min(0)]
    public float Speed = 1f;

    // PROPERTIES
    [Min(0)]
    public float AlignmentWeight = 1f;

    [Min(0)]
    public float CohesionWeight = 1f;

    [Min(0)]
    public float SeparationWeight = 1f;

    [Min(0)]
    public float SeparationRadius = 5f;

    // STEER
    [Min(0)]
    public float SteerWeight = 1f;

    [Min(0)]
    public float LeaderWeight = 1f;
}
