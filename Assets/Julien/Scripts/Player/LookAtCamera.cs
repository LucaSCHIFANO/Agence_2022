using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform camera;

    void Start()
    {
        camera = App.Instance.Session.Runner.GetPlayerObject(App.Instance.Session.Runner.LocalPlayer)
            .GetComponent<NetworkedPlayer>().Camera.transform;
    }

    void Update()
    {
        transform.LookAt(camera);
    }
}