using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MovementNOCCRB : NetworkBehaviour
{

    [SerializeField] private Transform localTransform;
    [SerializeField] private Transform distantTransform;
    [SerializeField] private List<Renderer> distantModel;
    [SerializeField] private GameObject Camera;
    
    
    [SerializeField] private Vector2 sensitivity = new Vector2(10, 10);

    [SerializeField] private float minimumY = -60;
    [SerializeField] private float maximumY = 60;

    [SerializeField] private float gravity;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    
    [SerializeField] private float slopeLimit;

    private Queue<ClientInputState> serverInputs = new Queue<ClientInputState>();

    private ClientInputState _inputState = new ClientInputState();
    private int simulationFrame;
    
    
    private int lastCorrectedFrame;
    private SimulationState serverSimulationState;
    
    public Vector3 velocity = Vector3.zero;

    private const int STATE_CACHE_SIZE = 1024;
    private SimulationState[] _simulationStateCache = new SimulationState[STATE_CACHE_SIZE];
    private ClientInputState[] _inputStateCache = new ClientInputState[STATE_CACHE_SIZE];
    
    
    private ClientInputState defaultInputState = new ClientInputState();
    
    private Quaternion originalRotation;
    private Quaternion originalCamRotation;

    private float rotationX;
    private float rotationY;
    
    private NetworkVariable<FixedString32Bytes> displayName = new NetworkVariable<FixedString32Bytes>();
    private NetworkVariable<int> selectedMaterial = new NetworkVariable<int>();
    private bool IsGrounded;

    private CapsuleCollider localCapsule;
    private CapsuleCollider serverCapsule;

    public bool IsMoving;

    void Start()
    {
        
        if (IsServer)
        {
            StartServer();
        }
        
        if (IsLocalPlayer)
        {
            StartLocalClient();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            UpdateLocalPlayer();
        }
    }

    void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            FixedUpdateClient();
        }

        if (IsServer)
        {
            FixedUpdateServer();
        }
    }

    void MovePlayer(Transform toMove, ClientInputState inputState)
    {
        if (!inputState.initialized)
        {
            inputState = defaultInputState;
        }

        IsMoving = inputState.pressedInput != Vector2.zero;

        if (IsServer)
        {
            toMove.rotation = inputState.rotation;
        }
        
        CheckGrounded(IsServer ? serverCapsule : localCapsule, toMove);
        if (IsGrounded)
        {
            velocity = (toMove.forward * inputState.pressedInput.x + toMove.right * inputState.pressedInput.y).normalized;
            velocity.y = 0f;
            velocity *= inputState.sprinting ? runSpeed : movementSpeed;
            
            if (inputState.jumped)
            {
                velocity.y = jumpForce;
            }
        }
        else
        {
            velocity.y -= gravity;
        }

        toMove.Translate(velocity, Space.World);
    }

    void CheckGrounded(CapsuleCollider capsuleCollider, Transform transform)
    {
        IsGrounded = false;
        float capsuleHeight = Mathf.Max(capsuleCollider.radius * 2f, capsuleCollider.height);
        Vector3 capsuleBottom = transform.TransformPoint(capsuleCollider.center - Vector3.up * capsuleHeight / 2f);
        float radius = transform.TransformVector(capsuleCollider.radius, 0f, 0f).magnitude;

        Ray ray = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, radius * 5f))
        {
            float normalAngle = Vector3.Angle(hit.normal, transform.up);
            if (normalAngle < slopeLimit)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                if (hit.distance < maxDist)
                    IsGrounded = true;
            }
        }
    }

    #region Client

    void StartLocalClient()
    {
        localCapsule = localTransform.GetComponent<CapsuleCollider>();
        // localRB = localTransform.GetComponent<Rigidbody>();
        // _userController = localTransform.GetComponent<CharacterController>();
        
        originalRotation = localTransform.rotation;
        originalCamRotation = Camera.transform.localRotation;
        
        
        localTransform.gameObject.SetActive(true);
        
        // distantTransform.gameObject.SetActive(false);
        distantModel.ForEach((renderer) => { renderer.enabled = false; });
    }
    
    void UpdateLocalPlayer()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivity.x;
        rotationY += Input.GetAxis("Mouse Y") * sensitivity.y;
        
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            
        localTransform.rotation = originalRotation * Quaternion.AngleAxis(rotationX, Vector3.up);
        Camera.transform.localRotation = originalCamRotation * Quaternion.AngleAxis(rotationY, -Vector3.right);
        
        _inputState = new ClientInputState
        {
            pressedInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")),
            rotation = localTransform.rotation,
            jumped = Input.GetButton("Jump"),
            sprinting = Input.GetKey(KeyCode.LeftShift),
            initialized = true
        };
    }

    void FixedUpdateClient()
    {
        _inputState.simulationFrame = simulationFrame;

        MovePlayer(localTransform, _inputState);

        SendInputStateServerRpc(_inputState);

        if (serverSimulationState.initialized) Reconciliate();

        SimulationState simulationState = CurrentSimulationState(localTransform, velocity, _inputState);

        int cacheIndex = simulationFrame % STATE_CACHE_SIZE;
        _simulationStateCache[cacheIndex] = simulationState;
        _inputStateCache[cacheIndex] = _inputState;
        
        ++simulationFrame;
    }

    void Reconciliate()
    {

        if (serverSimulationState.simulationFrame <= lastCorrectedFrame) return;
        
        int cacheIndex = serverSimulationState.simulationFrame % STATE_CACHE_SIZE;

        ClientInputState cachedInputState = _inputStateCache[cacheIndex];
        SimulationState cachedSimulationState = _simulationStateCache[cacheIndex];
        
        if (!cachedInputState.initialized || !cachedSimulationState.initialized)
        {
            localTransform.position = serverSimulationState.position;
            velocity = serverSimulationState.velocity;

            lastCorrectedFrame = serverSimulationState.simulationFrame;

            return;
        }

        float differenceX = Mathf.Abs(cachedSimulationState.position.x - serverSimulationState.position.x);
        float differenceY = Mathf.Abs(cachedSimulationState.position.y - serverSimulationState.position.y);
        float differenceZ = Mathf.Abs(cachedSimulationState.position.z - serverSimulationState.position.z);

        float tolerance = 0.0000001F;

        if (differenceX > tolerance || differenceY > tolerance || differenceZ > tolerance)
        {
            localTransform.position = serverSimulationState.position;
            velocity = serverSimulationState.velocity;

            int rewindFrame = serverSimulationState.simulationFrame;

            while (rewindFrame < simulationFrame)
            {
                int rewindCacheIndex = rewindFrame % STATE_CACHE_SIZE;

                ClientInputState rewindCachedInputState = _inputStateCache[rewindCacheIndex];
                SimulationState rewindCachedSimulationState = _simulationStateCache[rewindCacheIndex];

                if (!rewindCachedInputState.initialized || !rewindCachedSimulationState.initialized)
                {
                    ++rewindFrame;
                    continue;
                }

                MovePlayer(localTransform, rewindCachedInputState);

                SimulationState rewoundSimulationState =
                    CurrentSimulationState(localTransform, velocity, rewindCachedInputState);
                rewoundSimulationState.simulationFrame = rewindFrame;
                _simulationStateCache[rewindCacheIndex] = rewoundSimulationState;

                ++rewindFrame;
            }

        }

        lastCorrectedFrame = serverSimulationState.simulationFrame;
    }

    SimulationState CurrentSimulationState(Transform toMove, Vector3 velocity, ClientInputState inputState)
    {
        return new SimulationState
        {
            position = toMove.position,
            velocity = velocity,
            simulationFrame = inputState.simulationFrame,
            initialized = true
        };
    }

    [ServerRpc]
    void SendInputStateServerRpc(ClientInputState inputState)
    {
        if (!IsServer) return;

        serverInputs.Enqueue(inputState);
    }

    #endregion


    #region Server

    void StartServer()
    {
        // _serverController = distantTransform.GetComponent<CharacterController>();
        serverCapsule = distantTransform.GetComponent<CapsuleCollider>();
        // serverRB = distantTransform.GetComponent<Rigidbody>();
    }
    
    void FixedUpdateServer()
    {
        ClientInputState state;

        while (serverInputs.Count > 0 && (state = serverInputs.Dequeue()).initialized)
        {
            MovePlayer(distantTransform, state);

            SimulationState simState = CurrentSimulationState(distantTransform, velocity, state);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { OwnerClientId }
                }
            };
            
            ServerSimulationStateClientRpc(simState, clientRpcParams);
        }
    }

    [ClientRpc]
    void ServerSimulationStateClientRpc(SimulationState simulationState, ClientRpcParams clientRpcParams = default)
    {
        if (serverSimulationState.simulationFrame < simulationState.simulationFrame)
        {
            serverSimulationState = simulationState;
        }
    }

    #endregion
}
