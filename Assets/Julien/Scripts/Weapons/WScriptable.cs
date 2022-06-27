using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/SpawnNewWeapon")]
public class WScriptable : ScriptableObject
{
    public string turretName;
    
    public float fireRate;
    public float bulletToOverheat;
    public float coolDownPerSecond;
    public float timeBeforeCoolDown;
    public Color mainColor;
    public Color overHeatColor;

    public WeaponUltima.weapon wType;
    public WeaponFireType fType;

    public float maxDistanceRayCast;

    public GameObject shootingEffect;

    [Header("Basic / Burst / Shotgun / MachineGun")] 
    public float spread;
    public int numberOfShots;

    public float damage;
    public AudioClip weaponSound;

    public float timeBtwSound;


}
