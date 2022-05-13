using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PossessableWeapon : NetworkBehaviour
{
    private NetworkVariable<bool> isPossessed = new NetworkVariable<bool>(false);
    private PlayerController _playerController;
    [SerializeField] private Vector2 weaponSensibility;
    [SerializeField] private GameObject camera;

    private float timer;
    
    public void TryPossess(GameObject futurPossessor)
    {
        if (isPossessed.Value) return;
        
        ChangeOwnerServerRpc();
        _playerController = futurPossessor.GetComponent<PlayerController>();
        _playerController.Possess(gameObject);
        camera.SetActive(true);
        GetComponent<WeaponBase>().isPossessed = true;
        CanvasInGame.Instance.showOverheat(true);
        timer = 0.2f;
    }
    
    private void Update()
    {
        if (IsOwner && IsClient)
        {

            // activated = Input.GetKeyDown(activateDeactivate) ? !activated : activated;

            if (!isPossessed.Value) return;

            if (timer <= 0)
            {
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
            

            if (timer > 0)
                timer -= Time.deltaTime;
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
