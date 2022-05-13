using UnityEngine;

public class WeaponInteraction : Interactable
{
    [SerializeField] private string weaponName;

    public override string GetDescription()
    {
        return $"Press <color=green>[E]</color> to use the <i><color=#27f5f1>{weaponName}</color></i>";
    }

    public override void Interact(PlayerInteraction interactor)
    {
        // GetComponent<PossessableWeapon>().TryPossess(interactor.gameObject);
    }
}