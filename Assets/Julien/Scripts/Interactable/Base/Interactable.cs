using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class Interactable : NetworkBehaviour
{
    public enum InteractionType
    {
        Click,
        Hold
    }

    public InteractionType interactionType;

    public float requiredHoldTime;
    
    private float holdTime;

    public abstract string GetDescription();
    public abstract void Interact(PlayerInteraction interactor);

    public void IncreaseHoldTime() => holdTime += Time.deltaTime;
    public void ResetHoldTime() => holdTime = 0f;

    public float GetRequiredHoldTime() => requiredHoldTime;
    public float GetCurrentHoldTime() => holdTime;
}
