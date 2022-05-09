using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Testet : NetworkBehaviour
{

    [Header("Front wheels")]
    public GameObject leftFrontWheel;
    public GameObject rightFrontWheel;

    float currWheelDelta;
    Vector3 pos, prevPos;

    [Space(10)]
    [Header("Physics variables")]
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
    public float maxGripForce = 150;

    
    Vector3 prevRot;
    float wheelRadius = 0.1f;
    Vector3 angVel;
    float radius;
    bool drifting = false;
    private Rewired.Player player;

    Vector3 velocity;

    float acceleration;
    float maxAccel = 1;

    [Space(20)]
    [Header("Throttle")]
    [Range(0.0f, 1.0f)]
    public float bar;
    public float throttleAccelSpeed;
    public float throttleDeccelSpeed;
    public float accelForce;

    public AnimationCurve throttleForce;

    public GameObject remorque;



    public float remorqueTurningSpeed = 10;
    public float remorqueMaxDelta = 20;
    public float wheelRotSpeed;
    bool isRemorqueTurning = false;


    float delta;

    [Space(10)]
    [Header("Experimental")]
    public float sand = 5000;
    public float road = 10;
    
    
    void Start()
    {
        //if (!IsOwner) return;

        Debug.Log("Test get rewired");
        player = Rewired.ReInput.players.GetPlayer(0); // get the player by id
    }

    void Update()
    {
        //if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Space)) drifting = !drifting;
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (accelForce == sand) accelForce = road;
            else if (accelForce == road) accelForce = sand;
        }
        //Debug.Log(velocity);
        //Debug.Log(currWheelDelta);
    }

    private void FixedUpdate()
    {
        //if (!IsOwner) return;
        //Debug.Log("Fixed Update");

        if (drifting)
        {
            #region driftTemp
            Vector3 vec = transform.forward * player.GetAxis("Throttle");// Input.GetAxis("Vertical");

            //Debug.Log(vec);

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


            //Debug.Log("ANGULAR SPEED: " + angVel);
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


            if (rot == 0 && (angVel.y > -maxGripForce || angVel.y < maxGripForce)) drifting = false;

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

            //Debug.Log(fLatFront + "  " + fLatRear);

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
            #endregion
            #region temp
            /*
            //pos = transform.position;
            //Vector3 vec = transform.forward * player.GetAxis("Throttle");//Input.GetAxis("Vertical");
            //Vector3 dist = pos - prevPos;
            //Debug.Log(dist);
            //prevPos = pos;
            ///*
            //velocity = dist + vec / Time.deltaTime;
            //prevPos = pos;
            //
            //if (velocity.x > 30) velocity.x = 30;
            //velocity.y = 0;
            //if (velocity.z > 30) velocity.z = 30;
            //
            //
            //float speed = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);
            //
            //float wheelRotRate = speed / wheelRadius;
            //
            //angVel = (transform.rotation.eulerAngles - prevRot) / Time.deltaTime;
            //
            //
            //Debug.Log("ANGULAR SPEED: " + angVel);
            //prevRot = transform.rotation.eulerAngles;
            //
            //if (velocity.magnitude > 0)
            //    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, (transform.rotation.eulerAngles.y + currWheelDelta / 10), transform.rotation.eulerAngles.z);
            //Vector3 finalPos = transform.position + velocity * Time.deltaTime;
            ////Debug.Log(20 - (Mathf.Min(Mathf.Abs(currWheelDelta), 20)));
            //*/
            //
            //
            //
            //velocity = vec + dist / Time.deltaTime;
            //
            //Vector3 frr = -crr * velocity;
            //
            //float speed = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);
            //float wheelRotRate = speed / wheelRadius;
            //
            //
            //Vector3 fdrag;// = -cdrag * velocity * Vector3.Magnitude(velocity);
            //fdrag.x = -cdrag * velocity.x * speed;
            //fdrag.y = 0;//-cdrag * velocity.y * speed;
            //fdrag.z = -cdrag * velocity.z * speed;
            //
            //
            //Vector3 ftraction = transform.forward * enginePower;
            //
            //
            //Vector3 flong;
            //
            //flong = ftraction + fdrag + frr;
            //
            //
            //Vector3 accel = flong / weight;
            //
            //
            //velocity += Time.deltaTime * accel;
            //
            //velocity.y = 0;
            //
            //
            //Debug.Log("ANGULAR SPEED: " + angVel);
            //prevRot = transform.rotation.eulerAngles;
            //
            //if (velocity.magnitude > 0)
            //    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, (transform.rotation.eulerAngles.y + currWheelDelta / 10), transform.rotation.eulerAngles.z);
            //
            //Vector3 finalPos = transform.position + Time.deltaTime * velocity;
            //
            //transform.position = finalPos;
            //
            ////Debug.Log(velocity);
            //
            //
            //float rot = Input.GetAxis("Horizontal");
            //
            //currWheelDelta += rot;
            //
            ////if (angVel.y <= -maxGripForce || angVel.y >= maxGripForce) drifting = true;
            //
            //
            //if (currWheelDelta > 30) currWheelDelta = 30;
            //else if (currWheelDelta < -30) currWheelDelta = -30;
            //
            //if (rot == 0)
            //{
            //    if (currWheelDelta < 0)
            //    {
            //        currWheelDelta += (wheelRotRate / 10) * Time.deltaTime;
            //        if (currWheelDelta > 0) currWheelDelta = 0;
            //    }
            //    else if (currWheelDelta > 0)
            //    {
            //        currWheelDelta -= (wheelRotRate / 10) * Time.deltaTime;
            //        if (currWheelDelta < 0) currWheelDelta = 0;
            //    }
            //}
            //
            //leftFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
            //rightFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
            //*/
            #endregion
        }
        else
        {
            if (bar < 1) bar += player.GetAxis("Throttle") * (throttleAccelSpeed * Time.deltaTime);
            if (bar > 1) bar = 1;
            if (bar > 0) bar -= Time.deltaTime / throttleDeccelSpeed;
            if (bar < 0) bar = 0;

            pos = transform.position;
            Vector3 vec = transform.forward * throttleForce.Evaluate(bar);//Input.GetAxis("Vertical");
            Vector3 dist = pos - prevPos;

            if (acceleration < maxAccel && player.GetAxis("Throttle") > 0) acceleration += Time.deltaTime * accelForce;
            else if (acceleration > 0 && player.GetAxis("Throttle") == 0) acceleration -= Time.deltaTime * accelForce;

            //Debug.Log(acceleration);

            velocity = dist + vec / Time.deltaTime;
            prevPos = pos;

            if (velocity.x > 30) velocity.x = 30;
            velocity.y = 0;
            if (velocity.z > 30) velocity.z = 30;


            float speed = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);

            float wheelRotRate = speed / wheelRadius;

            angVel = (transform.rotation.eulerAngles - prevRot) / Time.deltaTime;


            //Debug.Log("ANGULAR SPEED: " + angVel);
            prevRot = transform.rotation.eulerAngles;

            Vector3 frr = -50 * velocity * speed;
            Vector3 breaking = player.GetAxis("Breaking") * -transform.forward * 10000;

            Vector3 fTot = frr + breaking;

            Vector3 accel = fTot / 1000;
            Debug.Log(accel);

            velocity.x += (transform.forward.x * acceleration) / Time.deltaTime;
            velocity.z += (transform.forward.z * acceleration) / Time.deltaTime;

            velocity += Time.deltaTime * accel;
            velocity.y = 0;

            if (velocity.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + (currWheelDelta / 20), transform.rotation.eulerAngles.z);
                if (delta < remorqueMaxDelta) 
                { 
                    remorque.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, (remorque.transform.rotation.eulerAngles.y - currWheelDelta / 10), transform.rotation.eulerAngles.z);
                    if (remorque.transform.rotation.eulerAngles.y != transform.rotation.eulerAngles.y)
                        isRemorqueTurning = true;
                }

            }
            Vector3 finalPos = transform.position + velocity * Time.deltaTime;
            //Debug.Log(speed);

            transform.position = finalPos;



            float rot = Input.GetAxis("Horizontal");

            currWheelDelta += rot;
            

            if (angVel.y <= -maxGripForce || angVel.y >= maxGripForce) drifting = true;


            if (currWheelDelta > 30) currWheelDelta = 30;
            else if (currWheelDelta < -30) currWheelDelta = -30;

            if (rot == 0)
            {
                if (currWheelDelta < 0)
                {
                    currWheelDelta += (wheelRotRate * wheelRotSpeed) * Time.deltaTime;
                    if (currWheelDelta > 0) currWheelDelta = 0;
                }
                else if (currWheelDelta > 0)
                {
                    currWheelDelta -= (wheelRotRate * wheelRotSpeed) * Time.deltaTime;
                    if (currWheelDelta < 0) currWheelDelta = 0;
                }
            }
            //Debug.Log(currWheelDelta);
            leftFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
            rightFrontWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, currWheelDelta, 0));
        }
        UpdateRemorque();
    }

    void UpdateRemorque()
    {
        /*
        pos = remorque.transform.position;
        Vector3 vec = remorque.transform.forward * throttleForce.Evaluate(bar);//Input.GetAxis("Vertical");
        Vector3 dist = pos - prevPos;

        if (acceleration < maxAccel && player.GetAxis("Throttle") > 0) acceleration += Time.deltaTime / 50;
        else if (acceleration > 0 && player.GetAxis("Throttle") == 0) acceleration -= Time.deltaTime / 50;

        //Debug.Log(acceleration);

        velocity = dist + vec / Time.deltaTime;
        prevPos = pos;

        if (velocity.x > 30) velocity.x = 30;
        velocity.y = 0;
        if (velocity.z > 30) velocity.z = 30;


        float speed = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);

        float wheelRotRate = speed / wheelRadius;

        angVel = (remorque.transform.rotation.eulerAngles - prevRot) / Time.deltaTime;


        //Debug.Log("ANGULAR SPEED: " + angVel);
        prevRot = remorque.transform.rotation.eulerAngles;
        
        velocity.x += (remorque.transform.forward.x * acceleration) / Time.deltaTime;
        velocity.z += (remorque.transform.forward.z * acceleration) / Time.deltaTime;
        */


        float dotSpeed = Vector3.Dot(remorque.transform.forward, transform.forward);
        delta = Mathf.Acos(dotSpeed / (remorque.transform.forward.magnitude * transform.forward.magnitude)) * 100;

        if (!isRemorqueTurning) return;
        if (remorque.transform.rotation.eulerAngles.y != transform.rotation.eulerAngles.y && velocity.magnitude > 0.0f && (Input.GetAxis("Horizontal") > -0.5f && Input.GetAxis("Horizontal") < 0.5f))
        {
            if (remorque.transform.rotation.eulerAngles.y > transform.rotation.eulerAngles.y)
                remorque.transform.rotation = Quaternion.Euler(remorque.transform.rotation.eulerAngles.x, remorque.transform.rotation.eulerAngles.y - Time.deltaTime * remorqueTurningSpeed, remorque.transform.rotation.eulerAngles.z);

            else if (remorque.transform.rotation.eulerAngles.y < transform.rotation.eulerAngles.y)
                remorque.transform.rotation = Quaternion.Euler(remorque.transform.rotation.eulerAngles.x, remorque.transform.rotation.eulerAngles.y + Time.deltaTime * remorqueTurningSpeed, remorque.transform.rotation.eulerAngles.z);
        }
        
        
        if (dotSpeed > 0.9999f && isRemorqueTurning)
        {
            remorque.transform.rotation = transform.rotation;
            isRemorqueTurning = false;
        }
        
        //if (velocity.magnitude > 0)
        //   remorque.transform.rotation = Quaternion.Euler(remorque.transform.rotation.eulerAngles.x, (remorque.transform.rotation.eulerAngles.y + currWheelDelta / 5), remorque.transform.rotation.eulerAngles.z);
        //Vector3 finalPos = remorque.transform.position + velocity * Time.deltaTime;
        //Debug.Log(20 - (Mathf.Min(Mathf.Abs(currWheelDelta), 20)));

            //remorque.transform.position = finalPos;

            //Debug.Log(velocity);

            /*
            float rot = Input.GetAxis("Horizontal");

            currWheelDelta += rot;

            //if (angVel.y <= -maxGripForce || angVel.y >= maxGripForce) drifting = true;


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
            */
    }

    
}