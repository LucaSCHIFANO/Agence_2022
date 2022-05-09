using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestControlWeapon : NetworkBehaviour
{

    [SerializeField] private KeyCode activateDeactivate;

    [SerializeField] bool activated;

    [SerializeField] private GameObject camera;

    private NetworkVariable<bool> isPossessed = new NetworkVariable<bool>(false);

    private PlayerController _playerController;

    [SerializeField] private float clampRotation;
    [SerializeField] private Vector2 weaponSensibility;
    

    private void Update()
    {
        if (IsOwner && IsClient)
        {

            // activated = Input.GetKeyDown(activateDeactivate) ? !activated : activated;

            if (!isPossessed.Value) return;

            float leftRight = Input.GetAxis("Mouse X") * weaponSensibility.x;
            float upDown = Input.GetAxis("Mouse Y") * weaponSensibility.y;

            transform.Rotate(Vector3.left, upDown);
            transform.Rotate(Vector3.up, leftRight);

            Vector3 eulerRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
            
            
            

            if (Input.GetMouseButton(0))
            {
                GetComponent<WeaponBase>().Shoot();
            }

            /*if (Input.GetKeyDown(KeyCode.R))
            {
                GetComponent<WeaponBase>().Reload();
            }*/

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Get Out");
                _playerController.enabled = true;
                camera.SetActive(false);
                _playerController.Unpossess();
                _playerController = null;
                Invoke(nameof(ResetOwner), .2f);
                GetComponent<WeaponBase>().isPossessed = false;
                CanvasInGame.Instance.showOverheat(false);
            }
        }
    }

    void ResetOwner()
    {
        RemoveOwnershipServerRpc();
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPossessed.Value) return;
        
        PlayerController playerController;
        if (other.gameObject.TryGetComponent(out playerController))
        {
            if (Input.GetKey(KeyCode.E))
            {
                ChangeOwnerServerRpc();
                playerController.Possess(gameObject);
                _playerController = playerController;
                camera.SetActive(true);
                GetComponent<WeaponBase>().isPossessed = true;
                CanvasInGame.Instance.showOverheat(true);
            }
        }
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
