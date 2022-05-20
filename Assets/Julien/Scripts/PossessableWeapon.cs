using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PossessableWeapon : NetworkBehaviour
{
    [Networked] private bool isPossessed { get; set; }
    private NetworkedPlayer _playerController;
    [SerializeField] private Vector2 weaponSensibility;
    [SerializeField] private GameObject camera;
    [SerializeField] private Transform exitPoint;

    private float timer;
    
    public void TryPossess(GameObject futurPossessor)
    {
        AskForPossessionServerRpc(Runner.LocalPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void AskForPossessionServerRpc(PlayerRef playerRef)
    {
        if (isPossessed) return;
        
        isPossessed = true;
        Object.AssignInputAuthority(playerRef);
        _playerController = Runner.GetPlayerObject(playerRef).gameObject.GetComponent<NetworkedPlayer>();
        _playerController.Possess(transform);
        // _playerController.transform.SetParent(transform);
        // _playerController.transform.localPosition = Vector3.zero;
        // _playerController.transform.localRotation = Quaternion.identity;
        camera.SetActive(true);
        GetComponent<WeaponBase>().isPossessed = true;
        // CanvasInGame.Instance.showOverheat(true);
        timer = 0.2f;
    }
    
    private void Update()
    {
        if (Object == null) return;
        
        if (Object.HasInputAuthority)
        {

            // activated = Input.GetKeyDown(activateDeactivate) ? !activated : activated;

            if (!isPossessed) return;

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
                    camera.SetActive(false);
                    // _playerController.transform.SetParent(null);
                    _playerController.Unpossess(exitPoint);
                    _playerController = null;
                    Invoke(nameof(ResetOwner), .2f);
                    GetComponent<WeaponBase>().isPossessed = false;
                    // CanvasInGame.Instance.showOverheat(false);
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

    [Rpc]
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
