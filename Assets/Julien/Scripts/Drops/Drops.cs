using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[Serializable]
public struct Drops
{
    [SerializeField] private string _dropName;
    [SerializeField] private bool _dropObject;
    [SerializeField] private NetworkObject _spawnedDropPrefab;
    [SerializeField] private float _ammountDropped;
    [SerializeField] private float _dropChance;
    [SerializeField] private bool _addFuel;
    [SerializeField] private bool _addScrap;

    public string DropName
    {
        get => _dropName;
        set => _dropName = value;
    }
    public bool DropObject
    {
        get => _dropObject;
        set => _dropObject = value;
    }
    public NetworkObject SpawnedDropPrefab
    {
        get => _spawnedDropPrefab;
        set => _spawnedDropPrefab = value;
    }
    public float AmountDropped
    {
        get => _ammountDropped;
        set => _ammountDropped = value;
    }
    public float DropChance
    {
        get => _dropChance;
        set => _dropChance = value;
    }
    public bool AddFuel
    {
        get => _addFuel;
        set => _addFuel = value;
    }
    public bool AddScrap
    {
        get => _addScrap;
        set => _addScrap = value;
    }
}
