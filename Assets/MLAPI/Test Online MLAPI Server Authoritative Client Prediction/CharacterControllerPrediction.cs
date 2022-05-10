using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CharacterControllerPrediction : NetworkBehaviour
{
    private static ClientInputState defaultInputState = new ClientInputState();

    private const int STATE_CACHE_SIZE = 1024;
    private SimulationState[] _simulationStateCache = new SimulationState[STATE_CACHE_SIZE];
    private ClientInputState[] _inputStateCache = new ClientInputState[STATE_CACHE_SIZE];

    private ClientInputState _inputState;
    Vector3 velocity = Vector3.zero;
    private int simulationFrame;

    private SimulationState serverSimulationState;
    private int lastCorrectedFrame;
    
    void Start()
    {
        if (IsLocalPlayer)
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ServerSimulationState", OnServerSimulationStateReceived);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            _inputState = new ClientInputState
            {
                pressedInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
                jumped = Input.GetButton("Jump")
            };
        }
    }

    void FixedUpdate()
    {
        if (!IsLocalPlayer) return;

        _inputState.simulationFrame = simulationFrame;
        
        ProcessInputs(_inputState);

        using FastBufferWriter writer = new FastBufferWriter(Marshal.SizeOf(_inputState) + 12, Allocator.Temp);
        writer.WriteNetworkSerializable(_inputState);
        
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ClientInputState", NetworkManager.ServerClientId, writer);
        
        if (serverSimulationState != null) Reconciliate();

        SimulationState simulationState = CurrentSimulationState(_inputState);

        int cacheIndex = simulationFrame % STATE_CACHE_SIZE;
        _simulationStateCache[cacheIndex] = simulationState;
        _inputStateCache[cacheIndex] = _inputState;
        
        ++simulationFrame;
    }

    #region Client
    
    public void ProcessInputs(ClientInputState state)
    {
        if (state != null)
        {
            state = defaultInputState;
        }

        velocity = (transform.forward * state.pressedInput.y + transform.right * state.pressedInput.x).normalized;
        velocity.y = 0f;

        transform.Translate(velocity);
    }
    
    public SimulationState CurrentSimulationState(ClientInputState inputState)
    {
        return new SimulationState
        {
            position = transform.position,
            velocity = velocity,
            simulationFrame = inputState.simulationFrame
        };
    }
    

    private void OnServerSimulationStateReceived(ulong senderClientId, FastBufferReader messagePayload)
    {
        messagePayload.ReadNetworkSerializable(out SimulationState message);
        
        if (serverSimulationState?.simulationFrame < message?.simulationFrame)
        {
            serverSimulationState = message;
        }
    }

    private void Reconciliate()
    {
        if (serverSimulationState.simulationFrame <= lastCorrectedFrame) return;

        int cacheIndex = serverSimulationState.simulationFrame % STATE_CACHE_SIZE;

        ClientInputState cachedInputState = _inputStateCache[cacheIndex];
        SimulationState cachedSimulationState = _simulationStateCache[cacheIndex];

        if (cachedInputState == null || cachedSimulationState == null)
        {
            transform.position = serverSimulationState.position;
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
            transform.position = serverSimulationState.position;
            velocity = serverSimulationState.velocity;

            int rewindFrame = serverSimulationState.simulationFrame;

            while (rewindFrame < simulationFrame)
            {
                int rewindCacheIndex = rewindFrame % STATE_CACHE_SIZE;
                
                ClientInputState rewindCachedInputState = _inputStateCache[rewindCacheIndex];
                SimulationState rewindCachedSimulationState = _simulationStateCache[rewindCacheIndex];
                
                if(rewindCachedInputState == null || rewindCachedSimulationState == null) 
                {
                    ++rewindFrame;
                    continue; 
                }
                
                ProcessInputs(rewindCachedInputState);
                
                SimulationState rewoundSimulationState = CurrentSimulationState(rewindCachedInputState);
                rewoundSimulationState.simulationFrame = rewindFrame; 
                _simulationStateCache[rewindCacheIndex] = rewoundSimulationState;
                
                ++rewindFrame;
            }
        }

        lastCorrectedFrame = serverSimulationState.simulationFrame;
    }

    #endregion
}
