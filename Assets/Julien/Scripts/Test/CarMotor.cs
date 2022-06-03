using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMotor : MonoBehaviour
{
    public Transform gravityTarget;

    public float power = 15000f;

    private float horInput;
    private float verInput;
    private float steerAngle;

    public Wheel[] wheels;

    void Update() 
    {
        ProcessInput();
    }

    void FixedUpdate()
    {
        ProcessForces();
    }

    void ProcessInput()
    {
        verInput = Input.GetAxis("Vertical");
        horInput = Input.GetAxis("Horizontal");
    }

    void ProcessForces()
    {
        foreach(Wheel w in wheels)
        {
            w.Steer(horInput);
            w.Accelerate(verInput * power);
            w.UpdatePosition();
        }
    }

}
