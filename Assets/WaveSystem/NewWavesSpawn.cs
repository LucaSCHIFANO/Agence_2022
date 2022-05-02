using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using Random = UnityEngine.Random;

public class NewWavesSpawn : MonoBehaviour
{
    public Transform target;
    public NewTestEnemy enemy;
    public List<float> listSphere = new List<float>();
    public bool spawn;

    private void Update()
    {
        if (spawn)
        {
            spawn = false;
            spawnEnemy(enemy);
        }
    }


    private void spawnEnemy(NewTestEnemy lenemy)
    {
        Vector3 spawnDistance = Random.onUnitSphere * lenemy.distanceSpawn;
        spawnDistance = new Vector3(spawnDistance.x, 0, spawnDistance.z);

        var enemyObject = Instantiate(lenemy.gameObject, spawnDistance, transform.rotation);
        enemyObject.GetComponent<NewTestEnemy>().target = target;

    }


    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < listSphere.Count; i++)
        {
            Gizmos.DrawWireSphere(transform.position, listSphere[i]);
        }
    }
}
