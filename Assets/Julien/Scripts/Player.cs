using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public PlayerCamera camera;
    Rewired.Player player;

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
        camera.player = this;
        player = Rewired.ReInput.players.GetPlayer(0);
        camera.Init();
    }


    private void Update()
    {
        if (player.GetButtonDown("ChangeCamView"))
            camera.ChangeCameraPosition();

        camera.UpdateCamera();
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