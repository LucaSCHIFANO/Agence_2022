using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class CharacterMovementPrediction : NetworkBehaviour
{

    [SerializeField] private Transform localTransform;
    [SerializeField] private Transform distantTransform;
    [SerializeField] private List<Renderer> distantModel;


    private Queue<ClientInputState> serverInputs = new Queue<ClientInputState>();

    private ClientInputState _inputState = new ClientInputState();
    private int simulationFrame;
    
    
    private int lastCorrectedFrame;
    private SimulationState serverSimulationState;
    
    Vector3 velocity = Vector3.zero;

    private const int STATE_CACHE_SIZE = 1024;
    private SimulationState[] _simulationStateCache = new SimulationState[STATE_CACHE_SIZE];
    private ClientInputState[] _inputStateCache = new ClientInputState[STATE_CACHE_SIZE];
    
    
    private ClientInputState defaultInputState = new ClientInputState();

    void Start()
    {
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

        velocity = (toMove.forward * inputState.pressedInput.x + toMove.right * inputState.pressedInput.y).normalized;
        velocity.y = 0f;
        
        toMove.Translate(velocity);
    }
    

    #region Client

    void StartLocalClient()
    {
        localTransform.gameObject.SetActive(true);

        distantModel.ForEach((renderer) => { renderer.enabled = false; });
    }
    
    void UpdateLocalPlayer()
    {
        _inputState = new ClientInputState
        {
            pressedInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")),
            jumped = Input.GetButton("Jump"),
            initialized = true
        };
    }

    void FixedUpdateClient()
    {
        _inputState.simulationFrame = simulationFrame;

        MovePlayer(localTransform, _inputState);

        Debug.Log("Sending input to server");
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

        Debug.Log("Server got inputs (adding to queue)");
        serverInputs.Enqueue(inputState);
    }

    #endregion


    #region Server

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
