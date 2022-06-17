using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CamionCarryFusion : SimulationBehaviour, IPlayerLeft
{
    public List<NetworkedPlayer> networkPlayer = new List<NetworkedPlayer>();

    public Vector3 lastPosition;
    public Vector3 lastRotation;
    private Transform _transform;

    // public Vector3 CurrentVelocity { get; private set; }

    void Start()
    {
        _transform = transform;
        lastPosition = _transform.position;
        lastRotation = _transform.eulerAngles;
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer)
        {
            if (networkPlayer.Count > 0)
            {
                Vector3 velocity = (_transform.position - lastPosition);
                Vector3 rotation = (_transform.eulerAngles - lastRotation);
                
                for (int i = 0; i < networkPlayer.Count; i++)
                {
                    NetworkedPlayer character = networkPlayer[i];
                    if (character == null)
                    {
                        networkPlayer.RemoveAt(i);
                        continue;
                    }

                    if (!character.IsInSomething)
                    {
                        character.transform.Translate(velocity, Space.World);
                        RotatePlayer(character, rotation.y);
                    }
                    /*if (!character.GetComponent<CharacterMovementHandler>().IsMoving)
                    {
                        character.GetComponent<NetworkCharacterControllerPrototypeCustom>().Move(velocity);
                    }*/
                }
            }

            lastPosition = _transform.position;
            lastRotation = _transform.eulerAngles;
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

    public void PlayerLeft(PlayerRef player)
    {
        RemovePlayer(App.Instance.GetPlayer(player).CharacterPrefab);
    }

    void RotatePlayer(NetworkedPlayer player, float amount)
    {
        player.transform.RotateAround(_transform.position, Vector3.up, amount);
    }
}