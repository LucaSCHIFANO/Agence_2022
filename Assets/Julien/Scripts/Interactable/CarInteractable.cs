using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CarInteractable : Interactable
{
    [SerializeField] private GameObject camera;
    [SerializeField] private Transform seat;
    [SerializeField] private Transform exit;
    
    public override string GetDescription()
    {
        return "Press <color=green>[E]</color> to drive the vehicule";
    }

    public override void Interact(PlayerInteraction interactor)
    {
        // AskForOwnershipServerRpc();
    }

    /*[Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    void AskForOwnershipServerRpc(RpcInfo info = default)
    {
        Movement camion = GetComponent<Movement>();
        if (camion.isPossessed) return;
        
        camion.Object.AssignInputAuthority(info.Source);
        camion.isPossessed = true;
        GameObject playerObject = Runner.GetPlayerObject(info.Source).gameObject;
        playerObject.transform.SetParent(transform);
        playerObject.transform.position = seat.position;
        playerObject.transform.rotation = seat.rotation;
        ConfirmPossessionClientRpc();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    void ConfirmPossessionClientRpc()
    {
        camera.SetActive(true);
        Runner.GetPlayerObject(Runner.LocalPlayer).GetComponent<NetworkedPlayer>().Possess(seat);
        GetComponent<Movement>().timer = .5f;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void AskForExitServerRpc(RpcInfo info = default)
    {
        Movement camion = GetComponent<Movement>();
        if (!camion.isPossessed) return;
        
        camion.Object.RemoveInputAuthority();
        camion.isPossessed = false;
        GameObject playerObject = Runner.GetPlayerObject(info.Source).gameObject;
        playerObject.transform.SetParent(null);
        playerObject.transform.position = exit.position;
        playerObject.transform.rotation = exit.rotation;
        ConfirmExitClientRpc(info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void ConfirmExitClientRpc([RpcTarget] PlayerRef target)
    {
        camera.SetActive(false);
        Runner.GetPlayerObject(Runner.LocalPlayer).GetComponent<NetworkedPlayer>().Unpossess(exit);
    }*/
}
