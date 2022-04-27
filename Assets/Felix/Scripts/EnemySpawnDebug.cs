using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnDebug : MonoBehaviour
{
    [SerializeField] private EnemySO enemy;

    private void Start()
    {
        if (enemy != null)
        {
            GameObject enemyGo = Instantiate(enemy.enemyPrefab, Vector3.zero, Quaternion.identity);
            enemyGo.GetComponent<Enemy>().Initialization(enemy);
        }
    }
}
