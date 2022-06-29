using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalProject : MonoBehaviour
{
    Camera cam;
    bool hitting = false;
    public GameObject decal;
    GameObject _decal;
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Debug.Break();
        if (Input.GetMouseButtonDown(0)) ApplyDecal();

        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            DeleteDecal();
            return;
        }
        GameObject hitGo = hit.collider.gameObject;

        if (hitGo.CompareTag("Remorque"))
        {
            if (!hitting)
            {
                hitting = true;
                _decal = Instantiate(decal, hit.point, Quaternion.Euler(hit.normal));
                _decal.transform.forward = -hit.normal;
                _decal.transform.parent = hitGo.transform;
                
            }
            else
            {
                _decal.transform.position = hit.point;
                //Debug.DrawRay(hit.point, hit.point + hit.normal, Color.red);
                //Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red);
                Quaternion rot = cam.transform.rotation;
                rot.x *= -1 * Mathf.Max(_decal.transform.rotation.x, 0);
                rot.y *= -1 * Mathf.Max(_decal.transform.rotation.y, 0);
                rot.z *= -1 * Mathf.Max(_decal.transform.rotation.z, 0);
                rot.w *= -1;
                //_decal.transform.rotation = rot;

            }
        }
        else DeleteDecal();
    }

    void DeleteDecal()
    {
        Destroy(_decal);
        hitting = false;
    }

    void ApplyDecal()
    {
        if (_decal != null) _decal = null;
        hitting = false;
    }
}
