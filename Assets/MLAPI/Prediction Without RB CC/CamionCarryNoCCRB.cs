using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CamionCarryNoCCRB : NetworkBehaviour
{
    public List<MovementNOCCRB> characterMovement = new List<MovementNOCCRB>();

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
                MovementNOCCRB character = characterMovement[i];
                Vector3 velocity = (_transform.position - lastPosition);
                Debug.Log(character.IsMoving);
                if (!character.IsMoving)
                    character.transform.Translate(velocity, _transform);
                // rb.MovePosition(rb.transform.position + velocity);
                // Debug.Log("Moving velocity " + velocity + " for rigidbody : " + rb.name);
                // rb.AddForce(velocity);
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
                
                if (rigidbody.transform.parent.TryGetComponent(out MovementNOCCRB prediction))
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
                if (rigidbody.transform.parent.TryGetComponent(out MovementNOCCRB prediction))
                    if (characterMovement.Contains(prediction))
                        characterMovement.Remove(prediction);
            }
        }
    }
}