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
                MovementPredictionCC character = characterMovement[i];
                Vector3 velocity = (_transform.position - lastPosition);
                Debug.Log(character.IsMoving);
                if (!character.IsMoving)
                    character.transform.Translate(velocity, _transform);
            }
        }

        lastPosition = _transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            Debug.Log(other);
            if (other.TryGetComponent(out CharacterController rigidbody))
            {
                Debug.Log(rigidbody);
                if (rigidbody.transform.parent.TryGetComponent(out MovementPredictionCC prediction))
                    if (!characterMovement.Contains(prediction))
                        characterMovement.Add(prediction);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsServer)
        {
            Debug.Log(other);
            if (other.TryGetComponent(out CharacterController rigidbody))
            {
                Debug.Log(rigidbody);
                if (rigidbody.transform.parent.TryGetComponent(out MovementPredictionCC prediction))
                    if (characterMovement.Contains(prediction))
                        characterMovement.Remove(prediction);
            }
        }
    }
}