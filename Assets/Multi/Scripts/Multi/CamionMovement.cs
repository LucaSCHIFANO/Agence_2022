using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CamionMovement : SimulationBehaviour
{
    [SerializeField] private Transform[] _movementPoint;
    [SerializeField] private float minDistance;
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rb;
    
    private int currentTarget;
    
    
    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer)
        {
            FixedUpdateServer();
        }
    }

    void FixedUpdateServer()
    {
        // Vector3 direction = (transform.position - _movementPoint[currentTarget].position).normalized;
        
        // rb.MovePosition(transform.position + (direction * speed * Runner.DeltaTime));
        transform.position = Vector3.MoveTowards(transform.position, _movementPoint[currentTarget].position, speed * Runner.DeltaTime);

        Vector3 dir = transform.position - _movementPoint[currentTarget].position;
        float length = dir.sqrMagnitude;

        if (length < minDistance * minDistance)
        {
            currentTarget++;

            if (currentTarget >= _movementPoint.Length) currentTarget = 0;
        }
    }
}
