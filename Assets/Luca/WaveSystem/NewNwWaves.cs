using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewNwWaves : NetworkBehaviour
{
    [Header("Tweekable")]
    //Nombre d'ennemis par vagues
    [SerializeField] private int minimEnemy;
    [SerializeField] private int maxEnemy;
    
    //Temps entre 2 vagues
    [SerializeField] private float minTimeBtwWaves;
    [SerializeField] private int maxTimeBtwWaves;
    
    //Temps entre 2 enemis 
    [SerializeField] private float minTimeBtwE;
    [SerializeField] private float maxTimeBtwE;
    
    //Random radius des spawn
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;

    [SerializeField] private List<int> enemyMultiplicator = new List<int>();
    [SerializeField] private List<Enemies.Enemy> enemyPool = new List<Enemies.Enemy>();
    
    [Header("Waves")]
    [SerializeField] private bool spawn;
    private int wavesCount = 0;
    private bool waveEnded;
    public List<Enemies.Enemy> enemiesSpawned = new List<Enemy>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9)) spawn = !spawn;
        
        if (spawn || Input.GetKeyDown((KeyCode.Return)))
        {
            spawn = false;
            waveEnded = false;

            if(Runner != null && Runner.IsServer) StartCoroutine(spawnEnemies());
        }
        
        if (waveEnded)
        {
            Debug.Log("ended");
            if(enemiesSpawned.Count == 0) Debug.Log("fin de vagues vraiment");
        }
    }
    
    IEnumerator spawnEnemies()
    {
        //yield return new WaitForSeconds(Random.Range(minTimeBtwE, maxTimeBtwE));
        var numberOfEnemies = Random.Range(minimEnemy, maxEnemy);
        
        for (int i = 0; i < numberOfEnemies; i++)
        {
            spawned();
            yield return new WaitForSeconds(Random.Range(minTimeBtwE, maxTimeBtwE));
        }
        
        waveEnded = true;
        
    }


    private void spawned()
    {
        Vector3 spawnDistance = RandomCircle(transform.position, Random.Range(minRadius, maxRadius));

        var randomEnemy = enemyPool[Random.Range(0, enemyPool.Count)];
        var enemyObject = Runner.Spawn(randomEnemy, spawnDistance, transform.rotation);
        enemyObject.SetWaves(this);
        enemiesSpawned.Add(enemyObject);
        

        //enemyObject.GetComponent<Enemy>().target = target;

    }

    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = UnityEngine.Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
    }

    public void removeEnemy(Enemies.Enemy toRemove)
    {
        enemiesSpawned.Remove(toRemove);
    }

    /*private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < listSphere.Count; i++)
        {
            Gizmos.DrawWireSphere(transform.position, listSphere[i]);
        }
        
        Gizmos.DrawWireSphere(player.position, minDistance);
    }*/
}
