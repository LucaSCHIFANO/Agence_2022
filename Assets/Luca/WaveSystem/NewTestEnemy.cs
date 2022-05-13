using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTestEnemy : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public float speed;
    public float distanceSpawn;
    

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            Destroy(gameObject);
        }
    }
}
