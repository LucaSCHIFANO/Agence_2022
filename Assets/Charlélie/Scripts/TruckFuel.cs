using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[ExecuteInEditMode]
public class TruckFuel : TruckBase
{
    #region DmgStuff
    [System.Serializable]
    public class ConstantDamageType
    {
        public string damageName;
        public float damagePercentagePerSecond;
        public bool isPersistant;
        public float duration;
        TruckFuel truck;
        public float currTime;
        //[HideInInspector]
        //public bool show;

        public void OnCreate(TruckFuel _truck)
        {
            currTime = duration;
            truck = _truck;
        }

        public void Update()
        {
            if (!isPersistant)
            {
                currTime -= Time.deltaTime;
                if (currTime <= 0)
                {
                    Delete();
                }
            }
            ApplyDamage();
        }

        public void Delete()
        {
            for (int i = 0; i < truck.currentDamagesApplied.Count; i++)
            {
                if (truck.currentDamagesApplied[i] == this)
                {
                    truck.currentDamagesApplied.Remove(truck.currentDamagesApplied[i]);
                }
            }
        }

        void ApplyDamage()
        {
            truck.currFuel -= ((damagePercentagePerSecond / truck.maxFuel) * 100) * Time.deltaTime;
        }
    }
    [HideInInspector]
    public List<ConstantDamageType> constantDamageType = new List<ConstantDamageType>();
    [HideInInspector]
    public List<ConstantDamageType> currentDamagesApplied = new List<ConstantDamageType>();

    public void AddConstDamage(string damageName)
    {
        Debug.Log(damageName);
        for (int i = 0; i < constantDamageType.Count; i++)
        {
            Debug.Log(constantDamageType[i].damageName);
            if (constantDamageType[i].damageName == damageName)
            {
                ConstantDamageType dmg = constantDamageType[i];
                Debug.Log(dmg.damageName);
                Debug.Log(dmg.damagePercentagePerSecond);
                dmg.OnCreate(this);
                currentDamagesApplied.Add(dmg);
                break;
            }
        }
    }

    public void AddConstDamage(ConstantDamageType damage)
    {
        Debug.Log(damage.damageName);
        Debug.Log(damage.damagePercentagePerSecond);
        ConstantDamageType dmg = damage;
        dmg.OnCreate(this);
        currentDamagesApplied.Add(dmg);
    }

    public void AddConstDamage(int index)
    {
        ConstantDamageType dmg = constantDamageType[index];
        dmg.OnCreate(this);
        currentDamagesApplied.Add(dmg);
    }

    public void AddDamageInList()
    {
        constantDamageType.Add(new ConstantDamageType());
        Debug.Log(constantDamageType.Count);
    }
    #endregion

    [HideInInspector]
    public float maxFuel;
    [HideInInspector]
    public float currFuel;

    [HideInInspector] [Networked] public float CurrFuelSync { get; set; }

    public float consPerMeterInL;
    TruckPhysics phys;
    
    public float totDist;
    float currMeter;
    Vector3 prevPos;
    public bool infiniteGas = false;

    [Networked] public bool OutOfGas { get; private set; }

    public override void Init()
    {
        currFuel = maxFuel;
        phys = GetComponent<TruckPhysics>();
    }

    public override void Spawned()
    {
        base.Spawned();
        CurrFuelSync = currFuel;
    }

    private void Update()
    {
        for (int i = 0; i < currentDamagesApplied.Count; i++)
        {
            currentDamagesApplied[i].Update();
        }

        if (Input.GetKeyDown(KeyCode.R)) currFuel = maxFuel;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;
        base.FixedUpdateNetwork();
        if (OutOfGas && currFuel > 0) OutOfGas = false;
        float currDist = Vector3.Distance(transform.position, prevPos);
        totDist += currDist;
        currMeter += currDist * phys.Throttle * (infiniteGas ? 0 : 1);
        if (currMeter >= 1)
        {
            currMeter = 0;
            currFuel -= consPerMeterInL;
        }
        prevPos = transform.position;
        CurrFuelSync = currFuel;
        if (!OutOfGas && currFuel <= 0) OutOfGas = true;
    }

    public void GetDamage(float value)
    {
        currFuel -= value;
    }

}
