using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CarInteractable : Interactable
{
    [SerializeField] private GameObject camera;
    [SerializeField] private Transform seat;
    [SerializeField] private Transform exit;
    
    [Networked] private bool isPossessed { get; set; }

    private float timer;
    
    public override string GetDescription()
    {
        return "Press <color=green>[E]</color> to drive the vehicule";
    }

    public override void Interact(PlayerInteraction interactor)
    {
        AskForOwnershipServerRpc();
    }

    private void Update()
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
        timer = .2f;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void AskForExitServerRpc(RpcInfo info = default)
    {
        if (!isPossessed) return;
        
        Object.RemoveInputAuthority();
        isPossessed = false;
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
    }
}
