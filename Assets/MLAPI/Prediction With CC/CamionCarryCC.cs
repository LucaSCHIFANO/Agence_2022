using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CamionCarryCC : NetworkBehaviour
{
    public List<MovementPredictionCC> characterMovement = new List<MovementPredictionCC>();

    public Vector3 lastPosition;
    private Transform _transform;

    public Vector3 CurrentVelocity { get; private set; }

    void Start()
    {
        _transform = transform;
        lastPosition = _transform.position;
    }

    void FixedUpdate()
    {
        Vector3 velocity = (_transform.position - lastPosition);
        CurrentVelocity = velocity;
        
        /*if (characterMovement.Count > 0)
        {
            for (int i = 0; i < characterMovement.Count; i++)
            {
                MovementPredictionCC character = characterMovement[i];
                
                if (!character.IsMoving)
                    character.transform.Translate(velocity, _transform);
            }
        }*/

        lastPosition = _transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.TryGetComponent(out CharacterController rigidbody))
            {
                Debug.Log("IN");
                if (rigidbody.transform.parent.TryGetComponent(out MovementPredictionCC prediction))
                {
                    prediction.CamionCarryCc = this;
                    if (!characterMovement.Contains(prediction))
                        characterMovement.Add(prediction);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsServer)
        {
            if (other.TryGetComponent(out CharacterController rigidbody))
            {
                Debug.Log("OUT");
                if (rigidbody.transform.parent.TryGetComponent(out MovementPredictionCC prediction))
                {
                    prediction.CamionCarryCc = null;
                    Debug.Log(prediction);
                    if (characterMovement.Contains(prediction))
                        characterMovement.Remove(prediction);
                }
            }
        }
    }
}