using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Action
{
    public PlayerActionType action;
    public CamType fpsCam, tpsCam;

    [HideInInspector]
    public CamType _currCam;
}

[System.Serializable]
public struct CamType
{
    public bool allow;
    public bool startWith;
    public CameraViewType camView;
    public Transform camTransform;
}

[System.Serializable]
public class PlayerCamera
{
    //EXPERIMENTAL
    [HideInInspector]
    public Player player;

    public AnimationCurve moveSpeed;

    public Camera cam;

    public PlayerActionType startAction;

    [HideInInspector]
    public Action currAction;

    public Action walk, drive, shoot;

    bool _isMoving = false;

    public void Init()
    {
        switch (startAction)
        {
            case PlayerActionType.WALKING:
                currAction = walk;
                break;

            case PlayerActionType.DRIVING:
                currAction = drive;
                break;

            case PlayerActionType.SHOOTING:
                currAction = shoot;
                break;
        }

        if (currAction.fpsCam.startWith) currAction._currCam = currAction.fpsCam;
        else if (currAction.tpsCam.startWith) currAction._currCam = currAction.tpsCam;
        else currAction._currCam = currAction.tpsCam;
        cam.transform.position = currAction._currCam.camTransform.position;
        cam.transform.rotation = currAction._currCam.camTransform.rotation;
    }

    public void ChangeCameraPosition()
    {
        if (_isMoving) return;

        switch (currAction._currCam.camView)
        {
            case CameraViewType.FPS:
                if (!currAction.tpsCam.allow) return;
                player.StartCamCoroutine(currAction._currCam.camTransform, currAction.tpsCam.camTransform);
                currAction._currCam = currAction.tpsCam;
                break;

            case CameraViewType.TPS:
                if (!currAction.fpsCam.allow) return;
                player.StartCamCoroutine(currAction._currCam.camTransform, currAction.fpsCam.camTransform);
                currAction._currCam = currAction.fpsCam;
                break;
        }

        
        
    }

    public IEnumerator CameraChangeCoroutine(Transform start, Transform end)
    {
        _isMoving = true;
        float index = 0;
        while (index < 1)
        {
            cam.transform.position = Vector3.Lerp(start.position, end.position, index);
            cam.transform.rotation = Quaternion.Lerp(start.rotation, end.rotation, index);
            index += Time.deltaTime * moveSpeed.Evaluate(index);
            yield return null;
        }
        _isMoving = false;
        yield return null;
    }

}
