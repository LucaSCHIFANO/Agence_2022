using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCar : MonoBehaviour
{
    public GameObject bullet;
    public float cdrag = 0.5f;
    public float crr = 0.5f;
    public float cbreak = 5;
    public float weight = 1;
    public float cornerStiff = 20;
    public float enginePower = 50;
    Vector3 velocity, prevVel;
    Vector3 pos, prevPos;
    Vector3 angVel;
    float prevRot, currRot;
    public float maxWheelDelta;
    float currWheelDelta;
    public GameObject leftFrontWheel, rightFrontWheel;

    float radius;

    public bool showDist, showFLong, showVel, showDrag, showRr, showSpeed;

    public bool freeMovement;

    void Start()
    {
        prevPos = transform.position;
        prevRot = transform.rotation.eulerAngles.y;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(bullet, transform.position + transform.forward * 5, transform.rotation);
        }
    }

    private void FixedUpdate()
    {
        if (freeMovement) return;

        Vector3 vec = transform.forward * Input.GetAxis("Vertical");
        float rot = Input.GetAxis("Horizontal");

        //transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y + rot, 0));
        currWheelDelta += rot;
        if (currWheelDelta > maxWheelDelta) currWheelDelta = maxWheelDelta;
        else if (currWheelDelta < -maxWheelDelta) currWheelDelta = -maxWheelDelta;
        leftFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
        rightFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
        radius = 1 / Mathf.Sin(currWheelDelta);

        

        Vector3 distance = transform.position - prevPos;
        prevPos = transform.position;

        if (showDist)
        Debug.Log("DISTANCE: " + distance);

        velocity = vec + distance / Time.deltaTime;


        //Debug.Log("ANGULAR SPEED: " + angularSpeed);
        prevRot = transform.rotation.eulerAngles.y;


        if (showVel)
        Debug.Log("VELOCITY: " + velocity);

        Vector3 frr = -crr * velocity;

        float speed = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);
        if (showSpeed) Debug.Log("SPEED: " + speed);


        float angularSpeed = speed / radius;//(transform.rotation.eulerAngles.y - prevRot) / Time.deltaTime;

        float longVel = Vector3.Magnitude(Mathf.Cos(currWheelDelta) * velocity);
        float latVel = Vector3.Magnitude(Mathf.Sin(currWheelDelta) * velocity);

        float slipAngleFront = Mathf.Atan((latVel + angularSpeed * 0.2f) / longVel) - Mathf.Sign(longVel);
        float slipAngleRear = Mathf.Atan((latVel - angularSpeed * 0.2f) / longVel);


        float fLatFront = cornerStiff * slipAngleFront;
        float fLatRear = cornerStiff * slipAngleRear;

        fLatFront = Mathf.Min(fLatFront, 5);
        fLatRear = Mathf.Min(fLatRear, 5);

        fLatFront *= 5;
        fLatRear *= 5;

        float torque = Mathf.Cos(currWheelDelta) * fLatFront * 0.2f - fLatRear * 0.2f;

        float angAccel = torque / 5;

        angularSpeed += angAccel * Time.deltaTime;

        Vector3 fdrag;// = -cdrag * velocity * Vector3.Magnitude(velocity);
        fdrag.x = -cdrag * velocity.x * speed;
        fdrag.y = 0;//-cdrag * velocity.y * speed;
        fdrag.z = -cdrag * velocity.z * speed;


      

        Vector3 fres = frr + fdrag;

        Vector3 ftraction = transform.forward * enginePower;

        
        Vector3 flong;

        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("---BREAKING---");
            Vector3 fbrake = -transform.forward * cbreak;
            flong = fbrake + fdrag + frr;
        }
        else
        {
            flong = ftraction + fdrag + frr;
        }

        if (showFLong)
        Debug.Log("FLONG: " + flong);

        if (showDrag)
            Debug.Log("DRAG: " + fdrag);

        if (showRr)
            Debug.Log("ROLLRES: " + frr);


        Vector3 accel = flong / weight;

        velocity += Time.deltaTime * accel;

        Vector3 finalPos = transform.position + Time.deltaTime * velocity;

        transform.position = finalPos;

        //Debug.Log(angularSpeed);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - angularSpeed * Time.deltaTime, transform.rotation.eulerAngles.z);
    }

    private void OnDrawGizmos()
    {
        if (currWheelDelta != 0)
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(transform.position, radius);

        }
    }
}
