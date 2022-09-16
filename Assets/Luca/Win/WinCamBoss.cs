using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCamBoss : MonoBehaviour
{
    [SerializeField] protected GameObject target;
    [SerializeField] protected float speed;

    private void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform);
            
            transform.RotateAround(target.transform.position, Vector3.up, speed);
        }
    }
}
