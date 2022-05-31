using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShopInteractable : Interactable
{
    // private PlayerController _playerController;
    [Networked] private bool isPossessed { get; set; }
    private float timer;
    
    public override string GetDescription()
    {
        return "Press <color=green>[E]</color> to open the shop";
    }

    public override void Interact(PlayerInteraction interactor)
    {
        //AskForOwnershipServerRpc();
        GetComponent<Shop>().Interact(interactor.GetComponent<NetworkedPlayer>());
    }
    
    /*private void Update()
    {
        if (isPossessed && Object.HasInputAuthority)
        {
            if (timer <= 0)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    AskForExitServerRpc();
                }
            }
        }

        if (timer > 0) timer -= Time.deltaTime;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    void AskForOwnershipServerRpc(RpcInfo info = default)
    {
        if (isPossessed) return;
        
        Object.AssignInputAuthority(info.Source);
        isPossessed = true;

        ConfirmPossessionClientRpc();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    void ConfirmPossessionClientRpc()
    {
        GetComponent<Shop>().Interact(Runner.GetPlayerObject(Runner.LocalPlayer).GetComponent<NetworkedPlayer>());
        timer = .2f;
    }
    

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void AskForExitServerRpc(RpcInfo info = default)
    {
        if (!isPossessed) return;

        Object.RemoveInputAuthority();
        isPossessed = false;
  
        ConfirmExitClientRpc(info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void ConfirmExitClientRpc([RpcTarget] PlayerRef target)
    {
        GetComponent<Shop>().quitShop();
    }*/
}
