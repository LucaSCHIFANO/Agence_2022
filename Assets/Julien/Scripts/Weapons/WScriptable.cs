using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/SpawnNewWeapon")]
public class WScriptable : ScriptableObject
{
    public float fireRate;
    public float bulletToOverheat;
    public float coolDownPerSecond;
    public float timeBeforeCoolDown;
    public Color mainColor;
    public Color overHeatColor;

    public WeaponUltima.weapon wType;
    public WeaponFireType fType;

    [Header("Basic / Burst / Shotgun / MachineGun")] 
    public float spread;
    public int numberOfShots;

    public float damage;


}
