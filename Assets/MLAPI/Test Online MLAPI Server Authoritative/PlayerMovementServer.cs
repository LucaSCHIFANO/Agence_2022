using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementServer : NetworkBehaviour
{
    [SerializeField] private GameObject Camera;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravity;

    [SerializeField] private float jumpForce;

    [SerializeField] private Vector2 sensitivity = new Vector2(15, 15);

    [SerializeField] private float minimumY = -60;
    [SerializeField] private float maximumY = 60;
    
    private Vector3 moveDirection;
    
    private Quaternion originalRotation;
    private Quaternion originalCamRotation;

    private float rotationX;
    private float rotationY;
    private Vector2  movement;
    
    private CharacterController _controller;
    
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        
        originalRotation = transform.rotation;
        originalCamRotation = Camera.transform.localRotation;

        if (IsLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Camera.SetActive(true);
        }
    }

    #region Online

    private NetworkVariable<Vector2> movementDirection = new NetworkVariable<Vector2>();
    private NetworkVariable<bool> isSprinting = new NetworkVariable<bool>();
    private NetworkVariable<bool> hasJumped = new NetworkVariable<bool>();

    private void Update()
    {
        if (IsLocalPlayer)
        {

            Debug.Log("Client Update");

            Vector2 newMovement = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            
            if (newMovement != movementDirection.Value)
            {
                MoveServerRpc(newMovement);
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
            }
            
            if (Input.GetKey(KeyCode.Space)) JumpServerRpc();

            if (isSprinting.Value != Input.GetKey(KeyCode.LeftShift))
                SprintServerRpc(Input.GetKey(KeyCode.LeftShift));

            // Look
            rotationX += Input.GetAxis("Mouse X") * sensitivity.x;
            rotationY += Input.GetAxis("Mouse Y") * sensitivity.y;

            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            
            CameraRotationServerRpc(originalRotation * Quaternion.AngleAxis(rotationX, Vector3.up));
            
            Camera.transform.localRotation = originalCamRotation * Quaternion.AngleAxis(rotationY, -Vector3.right);
        }
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 movement)
    {
        movementDirection.Value = movement;
    }

    [ServerRpc]
    private void SprintServerRpc(bool sprinting)
    {
        isSprinting.Value = sprinting;
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void CameraRotationServerRpc(Quaternion playerRotation)
    {
        transform.rotation = playerRotation;
    }

    [ServerRpc]
    private void JumpServerRpc()
    {
        hasJumped.Value = true;
    }

    private void FixedUpdate()
    {
        if (IsServer)
            MovePlayer();
    }

    void MovePlayer()
    {
        // Move
        if (_controller.isGrounded)
        {
            moveDirection = (transform.forward * movementDirection.Value.x + transform.right * movementDirection.Value.y).normalized;
            moveDirection.y = 0f;
            moveDirection *= isSprinting.Value ? runSpeed : movementSpeed;

            if (hasJumped.Value)
            {
                moveDirection.y = jumpForce;
            }
        }
        
        hasJumped.Value = false;
        
        // Gravity
        moveDirection.y -= gravity * Time.deltaTime;
        
        _controller.Move(moveDirection * Time.deltaTime);
    }
    

    #endregion
}
