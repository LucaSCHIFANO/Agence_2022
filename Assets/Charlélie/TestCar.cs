using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCar : MonoBehaviour
{
    public GameObject bullet;

    void Start()
    {
        
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
        Vector3 vec = new Vector3(0, 0, Input.GetAxis("Vertical"));
        float rot = Input.GetAxis("Horizontal");


        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y + rot, 0));
        transform.Translate(vec * 5 * Time.fixedDeltaTime);
    }
}
