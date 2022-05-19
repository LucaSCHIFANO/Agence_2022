using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private float destroyTimer;

    private void Start()
    {
        //if (IsServer)
            Destroy(gameObject, destroyTimer);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (IsServer)
            transform.position += transform.forward * velocity * Time.deltaTime;
    }

   /* private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"The bullet has touch a object : {other.gameObject.name}");
        Destroy(gameObject);
    }*/
}
