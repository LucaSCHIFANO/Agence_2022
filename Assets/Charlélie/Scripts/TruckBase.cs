using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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
