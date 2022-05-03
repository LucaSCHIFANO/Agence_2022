using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using Random = UnityEngine.Random;

public class NewWavesSpawn : MonoBehaviour
{
    [Header("Object")]
    public Transform player;
    public Transform target;
    
    [Header("Enemy")]
    public NewTestEnemy enemy;
    public List<float> listSphere = new List<float>();

    [Header("Other")] public float minDistance; 
    public bool spawn;

    private void Update()
    {
        if (spawn || Input.GetKeyDown((KeyCode.Return)))
        {
            spawn = false;
            spawnEnemy(enemy);
        }
    }


    private void spawnEnemy(NewTestEnemy lenemy)
    {
        Vector3 spawnDistance = transform.position + (Random.onUnitSphere * lenemy.distanceSpawn);
        spawnDistance = new Vector3(spawnDistance.x, 0, spawnDistance.z);

        while (Vector3.Distance(spawnDistance, player.position) < minDistance)
        {
            spawnDistance = transform.position + (Random.onUnitSphere * lenemy.distanceSpawn);
            spawnDistance = new Vector3(spawnDistance.x, 0, spawnDistance.z);
        }

        var enemyObject = Instantiate(lenemy.gameObject, spawnDistance, transform.rotation);
        enemyObject.GetComponent<NewTestEnemy>().target = target;

    }


    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < listSphere.Count; i++)
        {
            Gizmos.DrawWireSphere(transform.position, listSphere[i]);
        }
        
        Gizmos.DrawWireSphere(player.position, minDistance);
    }
}
