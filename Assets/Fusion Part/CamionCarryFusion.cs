using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CamionCarryFusion : SimulationBehaviour
{
    public List<NetworkPlayer> networkPlayer = new List<NetworkPlayer>();

    public Vector3 lastPosition;
    private Transform _transform;

    // public Vector3 CurrentVelocity { get; private set; }

    void Start()
    {
        _transform = transform;
        lastPosition = _transform.position;
    }
    
    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer)
        {
            Vector3 velocity = (_transform.position - lastPosition);
            // CurrentVelocity = velocity;

            if (networkPlayer.Count > 0)
            {
                for (int i = 0; i < networkPlayer.Count; i++)
                {
                    NetworkPlayer character = networkPlayer[i];
                    // character.transform.Translate(velocity, _transform);
                    if (!character.GetComponent<CharacterMovementHandler>().IsMoving)
                        character.GetComponent<NetworkCharacterControllerPrototypeCustom>().Move(velocity);
                }
            }

            lastPosition = _transform.position;
        }
    }

    public void AddPlayer(NetworkPlayer player)
    {
        if (!networkPlayer.Contains(player))
            networkPlayer.Add(player);
    }

    public void RemovePlayer(NetworkPlayer player)
    {
        if (networkPlayer.Contains(player))
            networkPlayer.Remove(player);
    }
}