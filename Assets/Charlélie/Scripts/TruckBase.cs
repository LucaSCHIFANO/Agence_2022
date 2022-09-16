using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TruckBase : NetworkBehaviour
{
    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
    }
}
