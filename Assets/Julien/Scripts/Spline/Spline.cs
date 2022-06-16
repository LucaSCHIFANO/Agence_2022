using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Spline : MonoBehaviour
{
    [SerializeField] private List<BezierSegment> _segments;
    [SerializeField] private Transform visual;
    [SerializeField] private float movementSpeed;
    [SerializeField] private bool restartWhenEnded;

    private float interpolateAmount;
    public int currentSegment = 0;

    private Vector3 previousPosition;
    
    private void Update()
    {
        interpolateAmount = (interpolateAmount + movementSpeed * Time.deltaTime);

        visual.position = CubicLerp(_segments[currentSegment].pointA, _segments[currentSegment].pointB,
            _segments[currentSegment].pointC, _segments[currentSegment].pointD, interpolateAmount);

        Vector3 moveDirection = visual.position - previousPosition;
        visual.rotation = Quaternion.LookRotation(moveDirection);

        if (visual.position == _segments[currentSegment].pointD)
        {
            if (currentSegment < _segments.Count-1)
            {
                currentSegment++;
                interpolateAmount = 0;
            } else if (restartWhenEnded)
            {
                currentSegment = 0;
                interpolateAmount = 0;
            }
        }
        previousPosition = visual.position;
    }

    Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }

    Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticLerp(a, b, c, t);
        Vector3 bc_cd = QuadraticLerp(b, c, d, t);

        return Vector3.Lerp(ab_bc, bc_cd, t);
    }
}

[Serializable]
public struct BezierSegment
{
    public Vector3 pointA;
    public Vector3 pointB;
    public Vector3 pointC;
    public Vector3 pointD;
}