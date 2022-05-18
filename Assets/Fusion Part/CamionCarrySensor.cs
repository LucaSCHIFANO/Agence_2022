using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamionCarrySensor : MonoBehaviour
{
    [SerializeField] private CamionCarryFusion _camionCarryFusion;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NetworkPlayer player))
        {
            _camionCarryFusion.AddPlayer(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NetworkPlayer player))
        {
            _camionCarryFusion.RemovePlayer(player);
        }
    }
}
