using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        }
    }

    
    public void addSpawerToList(WavesSpawner spawner)
    {
        activeSpawner.Add(spawner);
        Debug.Log(spawner.name + " added to list !!");
    }
    
    

    private void wavesLauncher()
    {
        var spawnerCount = currentWave._sides.Count * 2;
        
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
