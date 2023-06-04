using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vicsek Behaviour", menuName = "Vicsek/Behaviour", order = 1)]
public class VicsekBehaviour : ScriptableObject
{
    // DRIVE
    [Min(0)]
    public float Speed = 1f;

    // PROPERTIES
    [Min(0)]
    public float Noise = 0.1f;

    [Min(0)]
    public float AlignmentWeight = 1f;

    [Min(0)]
    public float AttractionWeight = 1f;

    [Min(0)]
    public float RepulsionWeight = 1f;

    [Min(0)]
    public float RepulsionRadius = 5f;

    // STEER
    [Min(0)]
    public float SteerWeight = 1f;

    [Min(0)]
    public float LeaderWeight = 1f;
}
