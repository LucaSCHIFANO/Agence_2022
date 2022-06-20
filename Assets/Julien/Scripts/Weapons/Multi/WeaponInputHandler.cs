using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class WeaponInputHandler : MonoBehaviour
{
    
    Vector2 viewInputVector = Vector2.zero;
    private bool isShooting;
    private bool isExiting;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1;
        
        isShooting = Input.GetButton("Fire1");
        isExiting = Input.GetKey(KeyCode.E);
    }

    public WeaponInputData GetNetworkInput()
    {
        WeaponInputData inputData = new WeaponInputData();

        inputData.rotationXInput = viewInputVector.x;
        inputData.rotationYInput = viewInputVector.y;

        inputData.isShooting = isShooting;
        inputData.isExiting = isExiting;

        return inputData;
    }
}
