using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CanBePossessed : NetworkBehaviour
{

    [SerializeField] private List<MonoBehaviour> scriptToActivate;
    [SerializeField] private List<GameObject> gameObjectsToActivate;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform mainSeat;

    private NetworkVariable<bool> isPossessed = new NetworkVariable<bool>(false);

    private PlayerController _playerNear;
    private PlayerController _playerController;

    private void Update()
    {
        if (IsOwner && IsClient)
        {
            if (isPossessed.Value)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    gameObjectsToActivate.ForEach((o) => { o.SetActive(false); });
                    scriptToActivate.ForEach((o) => { o.enabled = false; });
                    _playerController.enabled = true;
                    _playerController.Unpossess(exitPoint);
                    _playerController = null;
                    Invoke(nameof(ResetOwner), .2f);
                }
            }
        }

        if (!isPossessed.Value)
        {
            if (_playerNear != null)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    _playerController = _playerNear;
                    _playerController.Possess(mainSeat);
                    ChangeOwnerServerRpc();
                    gameObjectsToActivate.ForEach((o) => { o.SetActive(true); });
                    scriptToActivate.ForEach((o) => { o.enabled = true; });
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _playerNear = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPossessed.Value) return;
        
        
        if (other.gameObject.TryGetComponent(out PlayerController playerController))
        {
            _playerNear = playerController;
        }
    }

    void ResetOwner()
    {
        RemoveOwnershipServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    void ChangeOwnerServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!isPossessed.Value)
        {
            isPossessed.Value = true;
            NetworkManager.Singleton.ConnectedClients[rpcParams.Receive.SenderClientId].PlayerObject.transform.SetParent(transform);
            GetComponent<NetworkObject>().ChangeOwnership(rpcParams.Receive.SenderClientId);
        }
    }

    [ServerRpc]
    void RemoveOwnershipServerRpc(ServerRpcParams rpcParams = default)
    {
        if (isPossessed.Value)
        {
            isPossessed.Value = false;
            NetworkManager.Singleton.ConnectedClients[rpcParams.Receive.SenderClientId].PlayerObject.transform.SetParent(null);
            GetComponent<NetworkObject>().RemoveOwnership();
        }
    }
    
}
