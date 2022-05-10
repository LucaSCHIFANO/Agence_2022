using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementClient : NetworkBehaviour
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

    private CharacterController _controller;
    private Quaternion originalRotation;
    private Quaternion originalCamRotation;

    private float rotationX;
    private float rotationY;
    private bool isSprinting;
    private Vector2  movement;
    
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
    
    private void Update()
    {
        if (IsLocalPlayer)
        {
            // Debug.Log("Client Update");

            movement = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

            if (Input.GetKey(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
            }

            isSprinting = Input.GetKey(KeyCode.LeftShift);

            // Look
            rotationX += Input.GetAxis("Mouse X") * sensitivity.x;
            rotationY += Input.GetAxis("Mouse Y") * sensitivity.y;

            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            
            transform.rotation = originalRotation * Quaternion.AngleAxis(rotationX, Vector3.up);
            Camera.transform.localRotation = originalCamRotation * Quaternion.AngleAxis(rotationY, -Vector3.right);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // Move
        if (_controller.isGrounded)
        {
            moveDirection = (transform.forward * movement.x + transform.right * movement.y).normalized;
            moveDirection.y = 0f;
            moveDirection *= isSprinting ? runSpeed : movementSpeed;

            if (Input.GetKey(KeyCode.Space))
            {
                moveDirection.y = jumpForce;
            }
        }

        // Gravity
        moveDirection.y -= gravity * Time.deltaTime;
        
        _controller.Move(moveDirection * Time.deltaTime);
    }
}
