using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WavesSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRange;
    [SerializeField] private Side _side;
    public enum Side
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
    }

    public bool launch;
    public GameObject enemy;


    private void Start()
    {
        WavesManager.Instance.callForSpawners.AddListener(sendReadyToSpawn); 
    }

    private void sendReadyToSpawn(WavesManager.Waves currentWave)
    {
        var returnTrue = false;
        for (int i = 0; i < currentWave._sides.Count; i++)
        {
            if (currentWave._sides[i] == _side)
            {
                returnTrue = true;
                break;
            }   
        }

        if (returnTrue)
        {
            WavesManager.Instance.addSpawerToList(this);
        }
      
    }

    
    
    void Update()
    {
        if (launch)
        {
            launch = false;
            SpawnAnEnemy();
        }
    }
    
    
    

    void SpawnAnEnemy()
    {
        Vector3 spawnPoint = new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange + 1),
            transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange + 1));
        var enemy2 = Instantiate(enemy, spawnPoint, transform.rotation);
        enemy2.GetComponent<TestWaveEnemy>().target = WavesManager.Instance.target;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
