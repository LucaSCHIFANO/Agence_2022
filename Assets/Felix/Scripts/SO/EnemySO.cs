using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "SO/Enemy", order = 0)]
public class EnemySO : ScriptableObject
{
    public int health;
    // Weapons array to generate on the vehicle model
    public bool isKamikaze;
    [Space]
    public GameObject enemyPrefab;
}