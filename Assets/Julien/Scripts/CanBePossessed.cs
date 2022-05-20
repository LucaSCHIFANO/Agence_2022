using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CanBePossessed : NetworkBehaviour
{

    [SerializeField] private List<MonoBehaviour> scriptToActivate;
    [SerializeField] private List<GameObject> gameObjectsToActivate;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform mainSeat;

    [Networked] private bool isPossessed { get; set; }

    private PlayerController _playerNear;
    private PlayerController _playerController;

    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            if (isPossessed)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    gameObjectsToActivate.ForEach((o) => { o.SetActive(false); });
                    scriptToActivate.ForEach((o) => { o.enabled = false; });
                    _playerController.enabled = true;
                    // _playerController.Unpossess(exitPoint);
                    _playerController = null;
                    _playerNear = null;
                    Invoke(nameof(ResetOwner), .2f);
                }
            }
        }

        if (!isPossessed)
        {
            if (_playerNear != null)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    _playerController = _playerNear;
                    // _playerController.Possess(mainSeat);
                    ChangeOwnerServerRpc();
                    gameObjectsToActivate.ForEach((o) => { o.SetActive(true); });
                    scriptToActivate.ForEach((o) => { o.enabled = true; });
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.TryGetComponent(out PlayerController playerController))
        {
            _playerNear = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPossessed) return;
        
        
        if (other.gameObject.TryGetComponent(out PlayerController playerController))
        {
            _playerNear = playerController;
        }
    }

    void ResetOwner()
    {
        RemoveOwnershipServerRpc();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void ChangeOwnerServerRpc(RpcInfo rpcInfo = default)
    {
        if (!isPossessed)
        {
            isPossessed = true;
            App.Instance.GetPlayer(rpcInfo.Source).transform.SetParent(transform);
            Object.AssignInputAuthority(rpcInfo.Source);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RemoveOwnershipServerRpc(RpcInfo rpcInfo = default)
    {
        if (isPossessed)
        {
            isPossessed = false;
            App.Instance.GetPlayer(rpcInfo.Source).transform.SetParent(null);
            Object.RemoveInputAuthority();
        }
    }
    
}
