using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class EnemySpawnDebug : MonoBehaviour
{
    [SerializeField] private EnemySO enemy;

    private void Start()
    {
        if (enemy != null)
        {
            GameObject enemyGo = Instantiate(enemy.enemyPrefab, transform.position, Quaternion.identity);
            enemyGo.GetComponent<Enemy>().Initialization(enemy);
        }
    }
}
