using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Enemies;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using Random = UnityEngine.Random;

public class NewWavesSpawn : MonoBehaviour
{
    [Header("Object")]
    public Transform player;
    public Transform target;
    
    [Header("Enemy")]
    //public NewTestEnemy enemy;
    public List<float> listSphere = new List<float>();

    [Header("Other")] public float minDistance; 
    public bool spawn;
    
    
    
    [Header("Waves")]
    private int wavesCount = 0;
    private bool waveEnded;
    [HideInInspector] public NewWaves currentWave;
    [SerializeField] private List<NewWaves> _wavesList = new List<NewWaves>();
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9)) spawn = true;
        
        if (spawn || Input.GetKeyDown((KeyCode.Return)))
        {
            spawn = false;
            waveEnded = false;
            currentWave = _wavesList[wavesCount];

            StartCoroutine(spawnEnemies());
        }

        if (waveEnded)
        {
            if (wavesCount == _wavesList.Count - 1)
            {
                Debug.Log("end");
            }
            else
            {
                wavesCount++;
                spawn = true;
            }
        }
    }
    
    
    IEnumerator spawnEnemies()
    {
        yield return new WaitForSeconds(currentWave.delayBefore);
        
        for (int i = 0; i < currentWave.listEnemy.Count; i++)
        {
            spawned();
            yield return new WaitForSeconds(currentWave.delayBetwSpawn);
        }
        
        yield return new WaitForSeconds(currentWave.delayAfter);
        
        waveEnded = true;
        
    }


    private void spawned()
    {
        var wList = _wavesList[wavesCount];
        var lenemy = wList.listEnemy[wList.wavesCounter].enemy;

        for (int i = 0; i < lenemy.Count; i++)
        {
            Vector3 spawnDistance = RandomCircle(transform.position, listSphere[0]);

            while (Vector3.Distance(spawnDistance, player.position) < minDistance)
            {
                spawnDistance = RandomCircle(transform.position, listSphere[0]);
            }

            var enemyObject = Instantiate(lenemy[i].enemyPrefab, spawnDistance,
                transform.rotation);
            //enemyObject.GetComponent<Enemy>().target = target;

            
        }
        wList.wavesCounter++;
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
   


    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < listSphere.Count; i++)
        {
            Gizmos.DrawWireSphere(transform.position, listSphere[i]);
        }
        
        Gizmos.DrawWireSphere(player.position, minDistance);
    }
    
    
    [System.Serializable]
    public class NewWaves
    {
        public float delayBefore;
        public float delayBetwSpawn;
        public float delayAfter;
        
        public List<enemyList> listEnemy = new List<enemyList>();
        [HideInInspector] public int wavesCounter;
    }
    
    [System.Serializable]
    public class enemyList
    {
        public List<EnemySO> enemy = new List<EnemySO>();
    }
}
