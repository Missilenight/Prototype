using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlEngine : MonoBehaviour
{
    [Header("Simulation Properties")]

    [SerializeField, Tooltip("Total Agent Count")]
    int AGENT_COUNT = 0;

    [SerializeField, Tooltip("Agent Prefab")]
    ControlAgent AGENT_PREFAB;

    private List<ControlAgent> Agents = new List<ControlAgent>();

    private void Start()
    {
        // Get the total bounds of the gameObject
        Vector3 scale = transform.localScale;

        for (int i = 0; i < AGENT_COUNT; i++)
        {
            Vector3 spawnLocation = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;

            // For air animals they will spawn anywhere within the box
            spawnLocation = transform.position + new Vector3(Random.Range(-scale.x / 2, scale.x / 2), Random.Range(-scale.y / 2, scale.y), Random.Range(-scale.z / 2, scale.z / 2));

            float x = Random.Range(0f, 2f * Mathf.PI);
            float y = Random.Range(0f, 2f * Mathf.PI);
            float z = Random.Range(0f, 2f * Mathf.PI);

            spawnRotation = Quaternion.Euler(x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, z * Mathf.Rad2Deg);

            // Spawn the agent
            ControlAgent agent = Instantiate<ControlAgent>(AGENT_PREFAB, spawnLocation, spawnRotation);

            Agents.Add(agent);
        }
    }
}
