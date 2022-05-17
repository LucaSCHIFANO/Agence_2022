using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CamionCarryRB : NetworkBehaviour
{
    public List<MovementPredictionRB> characterMovement = new List<MovementPredictionRB>();

    public Vector3 lastPosition;
    private Transform _transform;

    void Start()
    {
        _transform = transform;
        lastPosition = _transform.position;
    }

    void FixedUpdate()
    {
        if (characterMovement.Count > 0)
        {
            for (int i = 0; i < characterMovement.Count; i++)
            {
                MovementPredictionRB character = characterMovement[i];
                Vector3 velocity = (_transform.position - lastPosition);
                if (!character.IsMoving)
                    character.transform.Translate(velocity, _transform);
            }
        }

        lastPosition = _transform.position;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (IsServer)
        {
            if (other.collider.TryGetComponent(out Rigidbody rigidbody))
            {
                
                if (rigidbody.transform.parent.TryGetComponent(out MovementPredictionRB prediction))
                    if (!characterMovement.Contains(prediction))
                        characterMovement.Add(prediction);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (IsServer)
        {
            if (other.collider.TryGetComponent(out Rigidbody rigidbody))
            {
                if (rigidbody.transform.parent.TryGetComponent(out MovementPredictionRB prediction))
                    if (characterMovement.Contains(prediction))
                        characterMovement.Remove(prediction);
            }
        }
    }
}