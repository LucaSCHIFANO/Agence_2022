using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorInteractable : Interactable
{
    // private PlayerController _playerController;
    
    public override string GetDescription()
    {
        return "Press <color=green>[E]</color> to open the generator panel";
    }

    public override void Interact(PlayerInteraction interactor)
    {
        // _playerController = interactor.GetComponent<PlayerController>();
        // CanvasInGame.Instance.showShop(true);
        // _playerController.enabled = false;
    }
}
