using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SubsystemsImplementation;
using Random = UnityEngine.Random;

public class WavesManager : MonoBehaviour
{
    [Header("List")]
    [SerializeField] private List<WavesSpawner> _wavesSpawners = new List<WavesSpawner>();
    [SerializeField] private List<Waves> _wavesList = new List<Waves>();
    
    private int wavesCount = 0;
    [HideInInspector] public Waves currentWave;
    
    //Event Spawn
    [HideInInspector] public MyWaveEvent callForSpawners = new MyWaveEvent();
    private List<WavesSpawner> activeSpawner = new List<WavesSpawner>();


    [Header("Other")]
    public Transform target;
    public bool startWave;
    public float valeurtest;

    #region Singleton
    private static WavesManager instance;
    public static WavesManager Instance { get => instance; set => instance = value; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        if (startWave)
        {
            startWave = false;
            currentWave = _wavesList[wavesCount];
            callForSpawners.Invoke(currentWave);
            
            wavesLauncher();
        }
    }

    
    
    public void addSpawerToList(WavesSpawner spawner)
    {
        activeSpawner.Add(spawner);
        //Debug.Log(spawner.name + " added to list !!");
    }
    
    

    private void wavesLauncher()
    {
        var spawnerCount = activeSpawner.Count;
        var enemyNumber = currentWave.enemyNumber;

        List<int> orderSpawn = new List<int>();

        var numerPerSpawnMin = Mathf.Floor(enemyNumber / spawnerCount);
        numerPerSpawnMin = Mathf.Ceil(numerPerSpawnMin / valeurtest);
        
        Debug.Log("minumum : " + numerPerSpawnMin);

        for (int i = 0; i < spawnerCount; i++)
        {
            for (int j = 0; j < numerPerSpawnMin; j++)
            {
                orderSpawn.Add(i); ;
            }
        }

        
        var count = orderSpawn.Count;
        for (int i = count; i < enemyNumber; i++)
        {
            orderSpawn.Add(Random.Range(0, spawnerCount));
        }

        for (int i = 0; i < orderSpawn.Count; i++)
        {
            Debug.Log(orderSpawn[i]);
        }
        
        orderSpawn = suffleList(orderSpawn);
        StartCoroutine(spawnEnemies(orderSpawn));

        
    }


    List<int> suffleList(List<int> alpha)
    {
        for (int i = 0; i < alpha.Count; i++) {
            int temp = alpha[i];
            int randomIndex = Random.Range(i, alpha.Count);
            alpha[i] = alpha[randomIndex];
            alpha[randomIndex] = temp;
        }

        return alpha;
    }


    IEnumerator spawnEnemies(List<int> order)
    {
        for (int i = 0; i < order.Count; i++)
        {
            activeSpawner[order[i]].launch = true;
            yield return new WaitForSeconds(currentWave.delayBetwSpawn);
        }
        
        wavesCount++;
    }





    [System.Serializable]
    public class Waves
    {
        public float enemyNumber;
        public float delayBetwSpawn;
        public List<WavesSpawner.Side> _sides = new List<WavesSpawner.Side>();
    }
    
    [System.Serializable]
    public class MyWaveEvent : UnityEvent<Waves>
    {
    }
    
}
