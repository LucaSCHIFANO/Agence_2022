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
    
    public void TryPossess(NetworkedPlayer other)
    {
        AskForPossessionServerRpc(Runner.LocalPlayer);
    }

    public void TryUnpossess()
    {
        AskForGetOutServerRpc();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void AskForPossessionServerRpc(PlayerRef playerRef)
    {
        if (isPossessed) return;
        
        isPossessed = true;
        Object.AssignInputAuthority(playerRef);
        _playerController = Runner.GetPlayerObject(playerRef).gameObject.GetComponent<NetworkedPlayer>();
        _playerController.transform.SetParent(transform);
        _playerController.Possess(transform);
        GetComponent<WeaponBase>().isPossessed = true;
        ConfirmPossessionClientRpc();
        SetParentingClientRpc();
        timer = 0.2f;
        
        if(_playerController != Runner.GetPlayerObject(Runner.LocalPlayer).gameObject.GetComponent<NetworkedPlayer>()) return;
        CanvasInGame.Instance.showOverheat(true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void ConfirmPossessionClientRpc()
    {
        camera.SetActive(true);
        _playerController = Runner.GetPlayerObject(Object.InputAuthority).gameObject.GetComponent<NetworkedPlayer>();
        _playerController.gameObject.GetComponent<CharacterMovementHandler>().enabled = false;
        _playerController.ChangeInputHandler(PossessingType.WEAPON, gameObject);
        // _playerController.gameObject.SetActive(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void SetParentingClientRpc()
    {
        Transform _playerTransform = Runner.GetPlayerObject(Object.InputAuthority).transform;
        _playerTransform.SetParent(transform);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void AskForGetOutServerRpc(RpcInfo rpcInfo = default)
    {
        if (!isPossessed) return;
        if (timer > 0) return;
        
        isPossessed = false;
        Object.RemoveInputAuthority();
        _playerController.transform.SetParent(null);
        _playerController.Unpossess(exitPoint);
        
        GetComponent<WeaponBase>().isPossessed = false;
        ConfirmGetOutClientRpc(rpcInfo.Source);
        
        if(_playerController != Runner.GetPlayerObject(Runner.LocalPlayer).gameObject.GetComponent<NetworkedPlayer>()) return;
        _playerController = null;
        CanvasInGame.Instance.showOverheat(false);
        
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void ConfirmGetOutClientRpc([RpcTarget] PlayerRef playerRef)
    {
        camera.SetActive(false);
        _playerController = Runner.GetPlayerObject(playerRef).gameObject.GetComponent<NetworkedPlayer>();
        _playerController.gameObject.GetComponent<CharacterMovementHandler>().enabled = true;
        _playerController.gameObject.SetActive(true);
        _playerController.transform.SetParent(null);
        _playerController.ChangeInputHandler(PossessingType.CHARACTER, gameObject);
        _playerController = null;
        // CanvasInGame.Instance.showOverheat(false);
    }

    private void Update()
    {
        if (Object == null) return;
        
        if (Object.HasStateAuthority)
        {

            // activated = Input.GetKeyDown(activateDeactivate) ? !activated : activated;

            // if (!isPossessed) return;

            // if (timer <= 0)
            // {
                /*
                float leftRight = Input.GetAxis("Mouse X") * weaponSensibility.x;
                float upDown = Input.GetAxis("Mouse Y") * weaponSensibility.y;

                transform.Rotate(Vector3.left, upDown);
                transform.Rotate(Vector3.up, leftRight);

                Vector3 eulerRotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
*/
                // if (Input.GetMouseButton(0))
                // {
                    // GetComponent<WeaponBase>().Shoot();
                // }

                /*if (Input.GetKeyDown(KeyCode.R))
                {
                    GetComponent<WeaponBase>().Reload();
                }*/

                // if (Input.GetKeyDown(KeyCode.E))
                // {
                    // Debug.Log("Get Out");
                    // AskForGetOutServerRpc();
                // }
            // }
            

            if (timer > 0)
                timer -= Time.deltaTime;
        }
    }
}
