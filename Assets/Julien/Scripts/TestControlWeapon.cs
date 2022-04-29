using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControlWeapon : MonoBehaviour
{

    [SerializeField] private KeyCode activateDeactivate;

    [SerializeField] bool activated;
    
    private void Update()
    {
        activated = Input.GetKeyDown(activateDeactivate) ? !activated : activated;

        if (!activated) return;
        
        float leftRight = Input.GetAxis("Horizontal");
        
        transform.Rotate(Vector3.up, leftRight);
        
        if (Input.GetMouseButton(0))
        {
            GetComponent<WeaponBase>().Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<WeaponBase>().Reload();
        }
    }
}
