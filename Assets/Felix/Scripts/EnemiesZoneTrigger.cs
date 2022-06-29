using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Fusion;
using UnityEngine;

public class EnemiesZoneTrigger : NetworkBehaviour
{
    private GameObject playerTruck;
    
    private Enemy[] enemies;
    private Vector3[] enemiesStartPosition;

    private bool isChasing;
    private float timer = 0f;
    
    [SerializeField] private float radius;
    [SerializeField] private LayerMask enemiesLayer;
    [Space]
    [SerializeField] private float timeStopChasing;
    

    public override void Spawned()
    {
        base.Spawned();
        
        Initialization();
    }

    private void Initialization()
    {
        if (!Runner.IsServer) return;
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemiesLayer);

        enemies = new Enemy[colliders.Length];
        enemiesStartPosition = new Vector3[colliders.Length];

        for (int i = 0; i < colliders.Length; i++)
        {
            enemies[i] = colliders[i].GetComponent<Enemy>();
            enemiesStartPosition[i] = colliders[i].transform.position;
        }
   
        playerTruck = GameObject.FindGameObjectWithTag("Car");

    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (timer > 0f)
        {
            timer -= Time.fixedDeltaTime;
        }
        else if (isChasing)
        {
            isChasing = false;
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].StopChasing(enemiesStartPosition[i]);
            }
        }
        
        if (!Runner.IsServer || playerTruck == null) return;

        if (Vector3.Distance(transform.position, playerTruck.transform.position) <= radius)
        {
            if (isChasing)
            {
                timer = timeStopChasing;
                return;
            }
            
            timer = timeStopChasing;
            isChasing = true;
            
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].Chase(playerTruck);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
