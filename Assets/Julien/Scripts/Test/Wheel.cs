using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] bool powered = false;
    [SerializeField] float maxAngle = 90f;
    [SerializeField] float offset = 0f;
    [SerializeField] Transform wheelVisual;
    [SerializeField] WheelCollider wheelCollider;

    private float turnAngle;

    public void Steer(float steerInput)
    {
        turnAngle = steerInput * maxAngle + offset;
        wheelCollider.steerAngle = turnAngle;
    }

    public void Accelerate(float powerInput)
    {
        if(powered) wheelCollider.motorTorque = powerInput;
        else wheelCollider.brakeTorque = 0;
    }

    public void UpdatePosition()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        wheelCollider.GetWorldPose(out pos, out rot);
        wheelVisual.position = pos;
        wheelVisual.rotation = rot;
    }
}
