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

    private CharacterController chara;
    private NetworkedPlayer playe;
    
    public bool IsMoving;
    private bool IsMovingOld;
    private bool isWasJumping;
    
    [HideInInspector] public NetworkMecanimAnimator anim;

    [Space(5)]



    [Header("Leak Params")]

    private GameObject _currLeakTargeted;

    private float _repairLeakCurrTime;



    [SerializeField] private float _repairLeakMaxTime;

    [SerializeField] private LayerMask _leakLayer;

    [SerializeField] private LayerMask partLayer;



    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        playe = GetComponent<NetworkedPlayer>();
        chara = GetComponent<CharacterController>();
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

        if (!playe.IsInSomething)
        {
            
            if (chara.isGrounded)
            {
                if (IsMoving && (!IsMovingOld || isWasJumping))
                {
                    IsMovingOld = true;
                    isWasJumping = false;
                    MySetTrigger("IsMoving");
                }
                else if (!IsMoving && (IsMovingOld || isWasJumping))
                {
                    IsMovingOld = false;
                    isWasJumping = false;
                    MySetTrigger("IsIdle");
                }
            }
            else if (!isWasJumping)
            {
                isWasJumping = true;
                MySetTrigger("Jump");
            }
        }
        else
        {
            MySetTrigger("Idle");
            Debug.Log("second step");
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

            if (networkInputData.isRepairing) CheckLeakReapir();
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

    void CheckLeakReapir()
    {
        RaycastHit hit;
        Physics.Raycast(localCamera.transform.position, localCamera.transform.forward, out hit, Mathf.Infinity, _leakLayer);
        if (hit.collider != null && (_currLeakTargeted == null | (_currLeakTargeted != hit.collider.gameObject)))
        {
            _repairLeakCurrTime = _repairLeakMaxTime;
            _currLeakTargeted = hit.collider.gameObject;
        }
        else if (hit.collider != null && _currLeakTargeted == hit.collider.gameObject)
        {
            _repairLeakCurrTime -= Time.deltaTime;
            if (_repairLeakCurrTime <= 0.0f)
                _currLeakTargeted.transform.parent.GetComponent<Leak>().OnDoneRepair();
        }

        else
        {
            _repairLeakCurrTime = _repairLeakMaxTime;
            _currLeakTargeted = null;
        }
    }

}
