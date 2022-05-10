using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
    [SerializeField] private Camera localCamera;

    private Vector2 viewInput;

    private float cameraRotationX;

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraRotationX += viewInput.y * Time.deltaTime * _networkCharacterControllerPrototypeCustom.rotationSpeedX;

        cameraRotationX = Mathf.Clamp(cameraRotationX, -60, 60);
        
        localCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            _networkCharacterControllerPrototypeCustom.Rotate(networkInputData.rotationInput);
            
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y +
                                    transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();
            
            _networkCharacterControllerPrototypeCustom.Move(moveDirection);
            
            if (networkInputData.isJumpPressed) _networkCharacterControllerPrototypeCustom.Jump();
        }
    }

    public void SetViewInputVector(Vector2 viewInputVector)
    {
        this.viewInput = viewInputVector;
    }
}
