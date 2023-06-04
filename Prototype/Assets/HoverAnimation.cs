using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAnimation : MonoBehaviour
{
    Vector3 spawnPos;
    float multi = .15f;

    private void Start()
    {
        spawnPos = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, spawnPos + new Vector3(0, multi * Mathf.Sin(Time.time)), 0.1f);
    }
}
