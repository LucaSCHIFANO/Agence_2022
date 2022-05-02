using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testet : MonoBehaviour
{
    public GameObject leftFrontWheel, rightFrontWheel;
    float currWheelDelta;
    Vector3 pos, prevPos;
    public float cdrag = 0.5f;
    public float crr = 0.5f;
    public float cbreak = 5;
    public float weight = 1;
    public float cornerStiff = 20;
    public float enginePower = 50;
    public float tyreLoad = 5000;
    public float wheelRotSped = 10;
    public float inertia = 10;
    public float maxWheelDelta = 30;
    Vector3 prevRot;
    float wheelRadius = 0.1f;
    Vector3 angVel;
    float radius;
    bool drifting = false;
    private Rewired.Player player;

    Vector3 velocity;
    void Start()
    {
        player = Rewired.ReInput.players.GetPlayer(0); // get the player by id
    }


    void Update()
    {
        Debug.Log(drifting);
        if (Input.GetKeyDown(KeyCode.Space)) drifting = !drifting;
        //Debug.Log(velocity);
        Debug.Log(currWheelDelta);
    }

    private void FixedUpdate()
    {
        if (drifting)
        {
            Vector3 vec = transform.forward * player.GetAxis("Throttle");// Input.GetAxis("Vertical");
            float rot = Input.GetAxis("Horizontal");

            //transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y + rot, 0));



            currWheelDelta += rot * wheelRotSped * Time.deltaTime;
            if (currWheelDelta > maxWheelDelta) currWheelDelta = maxWheelDelta;
            else if (currWheelDelta < -maxWheelDelta) currWheelDelta = -maxWheelDelta;
            //leftFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
            //rightFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
            radius = 1 / Mathf.Sin(currWheelDelta);

            //Debug.Log(pos + "   " + prevPos);
            pos = transform.position;
            Vector3 distance = pos - prevPos;
            prevPos = pos;


            velocity = vec + distance / Time.deltaTime;
            

            angVel = (transform.rotation.eulerAngles - prevRot) / Time.deltaTime;


            //Debug.Log("ANGULAR SPEED: " + angularSpeed);
            prevRot = transform.rotation.eulerAngles;

            Vector3 frr = -crr * velocity;

            float speed = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);

            float wheelRotRate = speed / wheelRadius;
            /*
            if (rot != 1)
            {
                if (currWheelDelta < 0) currWheelDelta += (wheelRotRate / 10) * Time.deltaTime;
                else if (currWheelDelta > 0) currWheelDelta -= (wheelRotRate / 10) * Time.deltaTime;
                //if (currWheelDelta < 0) currWheelDelta = 0;
                leftFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
                rightFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
                radius = 1 / Mathf.Sin(currWheelDelta);
            }
            */

            if (rot == 0)
            {
                if (currWheelDelta < 0)
                {
                    currWheelDelta += (wheelRotRate / 10) * Time.deltaTime;
                    if (currWheelDelta > 0) currWheelDelta = 0;
                }
                else if (currWheelDelta > 0)
                {
                    currWheelDelta -= (wheelRotRate / 10) * Time.deltaTime;
                    if (currWheelDelta < 0) currWheelDelta = 0;
                }
            }
            

            //if (rot == 0 && (currWheelDelta < 30.0f || currWheelDelta > -30.0f)) drifting = false;

            if (Input.GetKeyDown(KeyCode.E)) currWheelDelta = 0;
            leftFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
            rightFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
            radius = 1 / Mathf.Sin(currWheelDelta);

            float angularSpeed = speed / radius;//(transform.rotation.eulerAngles.y - prevRot) / Time.deltaTime;

            float longVel = Vector3.Magnitude(Mathf.Cos(currWheelDelta) * velocity);
            float latVel = Vector3.Magnitude(Mathf.Sin(currWheelDelta) * velocity);

            float slipAngleFront = Mathf.Atan((latVel + angularSpeed * 0.2f) / longVel) - Mathf.Sign(longVel);
            float slipAngleRear = Mathf.Atan((latVel - angularSpeed * 0.2f) / longVel);


            float fLatFront = cornerStiff * slipAngleFront;
            float fLatRear = cornerStiff * slipAngleRear;

            fLatFront = Mathf.Min(fLatFront, 5);
            fLatRear = Mathf.Min(fLatRear, 5);

            fLatFront *= tyreLoad;
            fLatRear *= tyreLoad;

            float torque = Mathf.Cos(currWheelDelta) * fLatFront * 0.2f - fLatRear * 0.2f;

            float angAccel = torque / inertia;

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
                if (speed < 0.1f) flong = Vector3.zero;
            }
            else
            {
                flong = ftraction + fdrag + frr;
            }


            Vector3 accel = flong / weight;


            velocity += Time.deltaTime * accel;

            velocity.y = 0;
            //velocity = leftFrontWheel.transform.forward * 5;

            if (Input.GetKeyDown(KeyCode.Space)) velocity = Vector3.zero;

            Vector3 finalPos = transform.position + Time.deltaTime * velocity;

            transform.position = finalPos;
            if (velocity.magnitude < 0.01f || speed < 0.1f) velocity = Vector3.zero;
            //Debug.Log(velocity.magnitude);

            float fRot = Mathf.Min(transform.rotation.eulerAngles.y + leftFrontWheel.transform.rotation.eulerAngles.y / 20, transform.rotation.eulerAngles.y + 5);
            //Debug.Log(transform.rotation.eulerAngles.y + leftFrontWheel.transform.rotation.eulerAngles.y / 20);
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, fRot, transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - angularSpeed * Time.deltaTime, transform.rotation.eulerAngles.z);
        }
        else
        {
            pos = transform.position;
            Vector3 vec = transform.forward * player.GetAxis("Throttle");//Input.GetAxis("Vertical");
            Vector3 dist = pos - prevPos;
            velocity = dist + vec / Time.deltaTime;
            prevPos = pos;

            if (velocity.x > 30) velocity.x = 30;
            if (velocity.z > 30) velocity.z = 30;


            float speed = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);

            float wheelRotRate = speed / wheelRadius;

            if (velocity.magnitude > 0)
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, (transform.rotation.eulerAngles.y + currWheelDelta / 10) * Mathf.Min(velocity.magnitude, 1), transform.rotation.eulerAngles.z);
            Vector3 finalPos = transform.position + /*transform.forward*/velocity * Time.deltaTime/* * (20 - (Mathf.Min(Mathf.Abs(currWheelDelta), 20)))*/;
            //Debug.Log(20 - (Mathf.Min(Mathf.Abs(currWheelDelta), 20)));

            transform.position = finalPos;

            float rot = Input.GetAxis("Horizontal");

            currWheelDelta += rot;

            //if (rot == 1 && (currWheelDelta >= 30.0f || currWheelDelta <= -30.0f)) drifting = true;


            if (currWheelDelta > 30) currWheelDelta = 30;
            else if (currWheelDelta < -30) currWheelDelta = -30;

            if (rot == 0)
            {
                if (currWheelDelta < 0)
                {
                    currWheelDelta += (wheelRotRate / 10) * Time.deltaTime;
                    if (currWheelDelta > 0) currWheelDelta = 0;
                }
                else if (currWheelDelta > 0)
                {
                    currWheelDelta -= (wheelRotRate / 10) * Time.deltaTime;
                    if (currWheelDelta < 0) currWheelDelta = 0;
                }
            }

            leftFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
            rightFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));

        }
    }
}
