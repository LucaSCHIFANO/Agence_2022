using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteractable : Interactable
{
    // private PlayerController _playerController;
    
    public override string GetDescription()
    {
        return "Press <color=green>[E]</color> to open the shop";
    }

    public override void Interact(PlayerInteraction interactor)
    {
        GetComponent<Shop>().Interact(interactor.GetComponent<NetworkedPlayer>());
    }
}
