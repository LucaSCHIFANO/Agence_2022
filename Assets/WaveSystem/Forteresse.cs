using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forteresse : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    public float timeToGo;
    private float VSpeed;

    private bool stop;
    
    void Start()
    {
        startPoint.parent = null;
        endPoint.parent = null;
        
        transform.position = startPoint.position;
        VSpeed = Vector3.Distance(startPoint.position, endPoint.position) / timeToGo;

        StartCoroutine(stopObject());
    }

    void Update()
    {
        if(!stop) transform.Translate((endPoint.position - startPoint.position).normalized * Time.deltaTime * VSpeed);
    }

    IEnumerator stopObject()
    {
        yield return new WaitForSeconds(timeToGo);
        stop = true;
    }
}
