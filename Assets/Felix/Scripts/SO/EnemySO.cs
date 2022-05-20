using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "SO/Enemy", order = 0)]
    public class EnemySO : ScriptableObject
    {
        public int health;
        public float speed;
        public float range;
        [Space] public GameObject[] weapons; // Weapons array to generate on the vehicle model // WeaponSO type
        [Space] public GameObject enemyPrefab;
    }
}