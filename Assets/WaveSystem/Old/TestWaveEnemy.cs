using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestWaveEnemy : MonoBehaviour
{
    public Transform target;
    public float speed;
    public WavesSpawner mySpawn;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            mySpawn.spawnedDeath();
            Destroy(gameObject);
        }
    }
}