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

    private Rewired.Player rPlayer;
    Vector2 angle;

    public void Init()
    {
        //angle = -1.57f; //To update
        rPlayer = Rewired.ReInput.players.GetPlayer(0);
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

    float speed = 2;

    public void UpdateCamera()
    {
        angle = new Vector2(rPlayer.GetAxis("MoveCamX"), rPlayer.GetAxis("MoveCamY"));

        Vector3 mySphericalCoord = SphericalMathHelpers.CartesianToSpherical(cam.transform.position);
        mySphericalCoord.z += angle.y * speed * Time.deltaTime;
        mySphericalCoord.y += angle.x * speed * Time.deltaTime;
        cam.transform.position = SphericalMathHelpers.SphericalToCartesian(mySphericalCoord);
        
        cam.transform.LookAt(player.transform.position);
    }

}


public static class SphericalMathHelpers
{

    public static Vector3 SphericalToCartesian(Vector3 sphericalCoord)
    {
        return SphericalToCartesian(sphericalCoord.x, sphericalCoord.y, sphericalCoord.z);
    }

    public static Vector3 SphericalToCartesian(float radius, float azimuth, float elevation)
    {

        float a = radius * Mathf.Cos(elevation);

        Vector3 result = new Vector3();
        result.x = a * Mathf.Cos(azimuth);
        result.y = radius * Mathf.Sin(elevation);
        result.z = a * Mathf.Sin(azimuth);

        return result;
    }

    /// <summary>
    /// Return Vector3 is x = radius, y = polar, z = elevation.
    /// </summary>
    /// <param name="cartCoords"></param>
    /// <returns></returns>
    public static Vector3 CartesianToSpherical(Vector3 cartCoords)
    {
        float _radius, _azimuth, _elevation;

        if (cartCoords.x == 0)
            cartCoords.x = Mathf.Epsilon;

        _radius = Mathf.Sqrt((cartCoords.x * cartCoords.x)
        + (cartCoords.y * cartCoords.y)
        + (cartCoords.z * cartCoords.z));

        _azimuth = Mathf.Atan(cartCoords.z / cartCoords.x);

        if (cartCoords.x < 0)
            _azimuth += Mathf.PI;
        _elevation = Mathf.Asin(cartCoords.y / _radius);

        Vector3 result = new Vector3(_radius, _azimuth, _elevation);
        return result;
    }
}
