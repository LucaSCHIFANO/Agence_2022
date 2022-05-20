using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TruckHealth : MonoBehaviour
{
    #region DmgStuff
    [System.Serializable]
    public class ConstantDamageType
    {
        public string damageName;
        public float damagePercentagePerSecond;
        public bool isPersistant;
        public float duration;
        TruckHealth truck;
        public float currTime;
        //[HideInInspector]
        //public bool show;

        public void OnCreate(TruckHealth _truck)
        {
            currTime = duration;
            truck = _truck;
        }

        public void Update()
        {
            currTime -= Time.deltaTime;
            if (currTime <= 0)
            {
                Delete();
            }
        }

        public void Delete()
        {
            for (int i = 0; i < truck.currentDamagesApplied.Count - 1; i++)
            {
                if (truck.currentDamagesApplied[i] == this)
                {
                    truck.currentDamagesApplied.Remove(truck.currentDamagesApplied[i]);
                }
            }
        }
    }
    public List<ConstantDamageType> constantDamageType = new List<ConstantDamageType>();
    [HideInInspector]
    public List<ConstantDamageType> currentDamagesApplied = new List<ConstantDamageType>();

    public void AddConstDamage(string damageName)
    {
        Debug.Log(damageName);
        for (int i = 0; i < constantDamageType.Count - 1; i++)
        {
            Debug.Log(constantDamageType[i].damageName);
            if (constantDamageType[i].damageName == damageName)
            {
                ConstantDamageType dmg = constantDamageType[i];
                Debug.Log(dmg.damageName);
                Debug.Log(dmg.damagePercentagePerSecond);
                dmg.OnCreate(this);
                currentDamagesApplied.Add(dmg);
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
    }
    #endregion

    void Start()
    {
        
    }


    void Update()
    {
        Debug.Log(constantDamageType.Count);
    }

    
}
