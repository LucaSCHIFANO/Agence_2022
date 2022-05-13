using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInteractable : Interactable
{
    public override string GetDescription()
    {
        return "Press <color=green>[E]</color> to drive the vehicule";
    }

    public override void Interact(PlayerInteraction interactor)
    {
    }
}