using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Leak : NetworkBehaviour
{
    [SerializeField] private GameObject _decal;
    [SerializeField] private GameObject _vfxFuelLeak;
    [SerializeField] private string damageName;
    public string DamageName { get => damageName; set => damageName = value; }
    public ReservoirParts.Part Part { get; set; }
    public TruckFuel.ConstantDamageType Damage { get => damage; set => damage = value; }
    private TruckFuel.ConstantDamageType damage;

    public GameObject VFXFuelLeak { get => _vfxFuelLeak; set => _vfxFuelLeak = value; }
    
    public override void Spawned()
    {
        
    }

    void Update()
    {
        
    }

    public void OnRepair()
    {

    }

    public void OnStopRepair()
    {

    }

    public void OnDoneRepair()
    {
        OnDoneRepairRpc();   
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    void OnDoneRepairRpc()
    {
        Debug.Log("HERE");
        TruckFuel[] t = FindObjectsOfType<TruckFuel>(); //TO CHANGE
        foreach (TruckFuel f in t)
        {
            f.StopDamage(ref damage);
        }
        Part.Repair();
    }
}
