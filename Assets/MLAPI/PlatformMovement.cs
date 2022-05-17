using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlatformMovement : NetworkBehaviour
{
    [SerializeField] private Transform[] _movementPoint;
    [SerializeField] private float minDistance;
    [SerializeField] private float speed;

    private int currentTarget;
    private void FixedUpdate()
    {
        if (IsServer)
        {
            FixedUpdateServer();
        }
    }

    void FixedUpdateServer()
    {
        transform.position = Vector3.MoveTowards(transform.position, _movementPoint[currentTarget].position, speed * Time.fixedDeltaTime);

        Vector3 dir = transform.position - _movementPoint[currentTarget].position;
        float length = dir.sqrMagnitude;

        if (length < minDistance * minDistance)
        {
            currentTarget++;

            if (currentTarget >= _movementPoint.Length) currentTarget = 0;
        }
    }
}
