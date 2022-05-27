using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponInteractable : Interactable
{
    public string weaponName;

    public override string GetDescription()
    {
        return $"Press <color=green>[E]</color> to use the <i><color=#27f5f1>{weaponName}</color></i>";
    }

    public override void Interact(PlayerInteraction interactor)
    {
        GetComponent<TestControlWeapon>().Interact(interactor.GetComponent<PlayerController>());
    }
}
