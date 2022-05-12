using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    
    [SerializeField] private GameObject Camera;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravity;

    [SerializeField] private float jumpForce;

    [SerializeField] private Vector2 sensitivity = new Vector2(15, 15);

    [SerializeField] private float minimumY = -60;
    [SerializeField] private float maximumY = 60;
    [SerializeField] private List<MeshRenderer> _meshRenderersSkin;
    [SerializeField] private TextMeshPro _usernameDisplay;
    [SerializeField] public List<Material> skinColor;

    private Vector3 moveDirection;

    private CharacterController _controller;
    private Quaternion originalRotation;
    private Quaternion originalCamRotation;

    private float rotationX;
    private float rotationY;
    private bool isSprinting;
    private Vector2  movement;

    private Vector3 possessPosition;
    private Quaternion possessRotation;
    private Quaternion cameraPossessRotation;
    
    private NetworkVariable<FixedString32Bytes> displayName = new NetworkVariable<FixedString32Bytes>();
    private NetworkVariable<int> selectedMaterial = new NetworkVariable<int>();

    [SerializeField] protected Animator anim;

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
            
            MakePlayerAnimServerRpc((moveDirection.x != 0 || moveDirection.z != 0));
            anim.SetBool("isWalking", (moveDirection.x != 0 || moveDirection.z != 0));
            // anim.gameObject.GetComponent<NetworkAnimator>().SetTrigger(0, true);
            // anim.gameObject.GetComponent<NetworkAnimator>().SetTrigger(1, true);
            
        }
        
        //if(IsClient) anim.SetBool("isWalking", (moveDirection.x != 0 || moveDirection.z != 0));
        
       
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

    public void Possess(GameObject gameObject)
    {
        possessPosition = transform.position;
        possessRotation = transform.rotation;
        cameraPossessRotation = Camera.transform.localRotation;

        GetComponent<Collider>().enabled = false;
        transform.position = gameObject.transform.position;
        transform.rotation = gameObject.transform.rotation;
        cameraPossessRotation = gameObject.transform.rotation;
        
        Camera.SetActive(false);

        enabled = false;
    }

    public void Possess(Transform seat)
    {
        GetComponent<Collider>().enabled = false;
        _controller.enabled = false;
        
        transform.position = seat.position;
        transform.rotation = seat.rotation;
        cameraPossessRotation = seat.rotation;
        
        Camera.SetActive(false);

        enabled = false;
    }
    

    public void Unpossess(Transform exitPoint)
    {
        Camera.SetActive(true);
        
        transform.position = exitPoint.position;
        transform.rotation = exitPoint.rotation;
        
        GetComponent<Collider>().enabled = true;
        _controller.enabled = true;
    }

    public void Unpossess()
    {
        GetComponent<Collider>().enabled = true;
        Camera.SetActive(true);
        
        transform.position = possessPosition;
        transform.rotation = possessRotation;
        Camera.transform.localRotation = cameraPossessRotation;
    }

    public void ChangeUsernameAndSkinColor(string username, Material skin)
    {
        _usernameDisplay.text = username;
        
        foreach (MeshRenderer renderer in _meshRenderersSkin)
        {
            renderer.material = skin;
        }
    }
    
    private void Awake()
    {
        Debug.Log("Enable Player Controller");
        displayName.OnValueChanged += HandleDisplayNameChanged;
        selectedMaterial.OnValueChanged += HandleSkinChanged;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        PlayerData? playerData = RelayManager.GetPlayerData(OwnerClientId);
        
        if (playerData.HasValue)
        {
            displayName.Value = playerData.Value.Username;
            selectedMaterial.Value = playerData.Value.ChoosenColor;
            Debug.Log(playerData.Value.ChoosenColor);
            Debug.Log("Username Changed");
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        displayName.OnValueChanged -= HandleDisplayNameChanged;
        selectedMaterial.OnValueChanged -= HandleSkinChanged;
        Debug.Log("Disable Rows");
    }

    void HandleDisplayNameChanged(FixedString32Bytes oldDisplayName, FixedString32Bytes newDisplayName)
    {
        Debug.Log(newDisplayName);
        _usernameDisplay.text = newDisplayName.ToString();
    }

    void HandleSkinChanged(int oldColorSkin, int newColorSkin)
    {
        Debug.Log("new color change " + newColorSkin);
        foreach (MeshRenderer renderer in _meshRenderersSkin)
        {
            renderer.material = skinColor[newColorSkin];
        }
    }

    [ServerRpc]
    void MakePlayerAnimServerRpc(bool isWalking)
    {
        if (IsOwner && IsClient) return;
        
        anim.SetBool("isWalking", isWalking);
    }
    
/* Server Authoritative (faut regarder comment forcer le déplacement depuis le client)
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
    }*/
}
