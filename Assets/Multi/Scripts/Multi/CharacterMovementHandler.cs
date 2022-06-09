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

    public bool IsMoving;
    private bool IsMovingOld;
    
    [SerializeField] protected NetworkMecanimAnimator anim;
    

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    // Start is called before the first frame update
    void Start()
    {
        IsMoving = true;
        IsMovingOld = false;
    }

    // Update is called once per frame
    void Update()
    {
        cameraRotationX += viewInput.y * Time.deltaTime * _networkCharacterControllerPrototypeCustom.rotationSpeedX;

        cameraRotationX = Mathf.Clamp(cameraRotationX, -60, 60);
        
        localCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);

        if (IsMoving && !IsMovingOld)
        {
            IsMovingOld = true;
            MySetTrigger("IsMoving");
        }
        else if (!IsMoving && IsMovingOld)
        {
            IsMovingOld = false;
            MySetTrigger("IsMoving");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            _networkCharacterControllerPrototypeCustom.Rotate(networkInputData.rotationXInput);
            
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y +
                                    transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            IsMoving = moveDirection != Vector3.zero || networkInputData.isJumpPressed;
            
            _networkCharacterControllerPrototypeCustom.Move(moveDirection);
            
            if (networkInputData.isJumpPressed) _networkCharacterControllerPrototypeCustom.Jump();
            
            if (networkInputData.isRequestingToSpawn) SpawnerVehicule.instance.SpawnCar();
        }
    }

    public void SetViewInputVector(Vector2 viewInputVector)
    {
        this.viewInput = viewInputVector;
    }
    
    
    public void MySetTrigger(string trigger)
    {
        if (Object.HasStateAuthority)
            anim.SetTrigger(trigger);
        else if (Object.HasInputAuthority && Runner.IsForward)
            anim.Animator.SetTrigger(trigger);
    }
}
