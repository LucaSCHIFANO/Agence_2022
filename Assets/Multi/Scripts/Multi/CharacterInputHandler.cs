using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    private bool isJumpButtonPressed;
    private bool isRequestingToSpawn;

    private bool isRepairing;
    private bool isShooting;

    private CharacterMovementHandler _characterMovementHandler;

    private void Awake()
    {
        _characterMovementHandler = GetComponent<CharacterMovementHandler>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1;

        _characterMovementHandler.SetViewInputVector(viewInputVector);
        
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        isJumpButtonPressed = Input.GetButton("Jump");
        isRequestingToSpawn = Input.GetKeyDown(KeyCode.P);

        isShooting = Input.GetMouseButtonDown(0);
        isRepairing = Input.GetMouseButton(1);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData inputData = new NetworkInputData();

        inputData.rotationXInput = viewInputVector.x;
        inputData.rotationYInput = viewInputVector.y;

        inputData.movementInput = moveInputVector;

        inputData.isJumpPressed = isJumpButtonPressed;
        inputData.isRequestingToSpawn = isRequestingToSpawn;

        inputData.isRepairing = isRepairing;
        inputData.isShooting = isShooting;

        return inputData;
    }
}
