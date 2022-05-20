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
        _playerController.transform.SetParent(transform);
        _playerController.Possess(transform);
        GetComponent<WeaponBase>().isPossessed = true;
        ConfirmPossessionClientRpc();
        SetParentingClientRpc();
        // CanvasInGame.Instance.showOverheat(true);
        timer = 0.2f;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void ConfirmPossessionClientRpc()
    {
        camera.SetActive(true);
        _playerController = Runner.GetPlayerObject(Object.InputAuthority).gameObject.GetComponent<NetworkedPlayer>();
        _playerController.gameObject.GetComponent<CharacterMovementHandler>().enabled = false;
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
        
        isPossessed = false;
        Object.RemoveInputAuthority();
        _playerController.transform.SetParent(null);
        _playerController.Unpossess(exitPoint);
        _playerController = null;
        GetComponent<WeaponBase>().isPossessed = false;
        // CanvasInGame.Instance.showOverheat(false);
        ConfirmGetOutClientRpc(rpcInfo.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void ConfirmGetOutClientRpc([RpcTarget] PlayerRef playerRef)
    {
        Debug.Log(playerRef);
        camera.SetActive(false);
        _playerController = Runner.GetPlayerObject(playerRef).gameObject.GetComponent<NetworkedPlayer>();
        _playerController.gameObject.GetComponent<CharacterMovementHandler>().enabled = true;
        _playerController.gameObject.SetActive(true);
        _playerController.transform.SetParent(null);
        _playerController = null;
        // CanvasInGame.Instance.showOverheat(false);
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
                /*
                float leftRight = Input.GetAxis("Mouse X") * weaponSensibility.x;
                float upDown = Input.GetAxis("Mouse Y") * weaponSensibility.y;

                transform.Rotate(Vector3.left, upDown);
                transform.Rotate(Vector3.up, leftRight);

                Vector3 eulerRotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
*/
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
                    AskForGetOutServerRpc();
                }
            }
            

            if (timer > 0)
                timer -= Time.deltaTime;
        }
    }
}
