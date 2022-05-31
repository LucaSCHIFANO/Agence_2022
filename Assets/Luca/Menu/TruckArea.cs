using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckArea : MonoBehaviour
{
     public List<GameObject> objectInAreaTruck = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(!objectInAreaTruck.Contains(other.gameObject)) objectInAreaTruck.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        objectInAreaTruck.Remove(other.gameObject);
    }
}
