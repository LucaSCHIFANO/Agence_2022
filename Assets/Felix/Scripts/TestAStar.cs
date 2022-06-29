using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    private Asker asker;
    
    [SerializeField] private Transform targetTransform;

    private void Awake()
    {
        asker = GetComponent<Asker>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            asker.AskNewPath(targetTransform.position, 0, null, true);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            asker.AskNewPath(targetTransform.position, 0, null, false);
        }
    }
}
