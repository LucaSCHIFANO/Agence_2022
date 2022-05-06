using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public PlayerCamera camera;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        Debug.Log("Spawned in game");

        if (IsOwner)
        {
            camera.cam.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        if (IsOwner && IsClient)
        {
            camera.player = this;
            camera.Init();
        }
    }
  

    private void Update()
    {
        if (IsOwner && IsClient)
        {
            //TEMPORARY
            if (Input.GetKeyDown(KeyCode.C))
                camera.ChangeCameraPosition();

            camera.UpdateCamera();
        }
    }

    public void StartCamCoroutine(Transform start, Transform end)
    {
        StartCoroutine(camera.CameraChangeCoroutine(start, end));
    }

    public void PlayerChangeAction()
    {
        //TODO
    }
}
