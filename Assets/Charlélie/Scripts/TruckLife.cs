using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckLife : TruckBase
{
    public float maxLife;
    public float currLife;


    public override void Init()
    {
        currLife = maxLife;
    }

    void Start()
    {
        Init();
    }


    void Update()
    {
        
    }
}
