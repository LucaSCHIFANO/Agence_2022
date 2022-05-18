using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CamionCarryFusion : SimulationBehaviour
{
    public List<NetworkedPlayer> networkPlayer = new List<NetworkedPlayer>();

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
                    NetworkedPlayer character = networkPlayer[i];
                    // character.transform.Translate(velocity, _transform);
                    if (!character.GetComponent<CharacterMovementHandler>().IsMoving)
                        character.GetComponent<NetworkCharacterControllerPrototypeCustom>().Move(velocity);
                }
            }

            lastPosition = _transform.position;
        }
    }

    public void AddPlayer(NetworkedPlayer player)
    {
        if (!networkPlayer.Contains(player))
            networkPlayer.Add(player);
    }

    public void RemovePlayer(NetworkedPlayer player)
    {
        if (networkPlayer.Contains(player))
            networkPlayer.Remove(player);
    }
}