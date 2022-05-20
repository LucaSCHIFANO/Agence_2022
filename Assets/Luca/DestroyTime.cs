using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    public float timeToDelete;
    void Start()
    {
        Destroy(gameObject, timeToDelete);
    }

}
