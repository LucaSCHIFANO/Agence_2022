using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AutoLock : NetworkBehaviour
{
    [SerializeField] private float lockingDistance;
    [SerializeField] private Transform VehicleTargets;
    [SerializeField] private bool prioritizeVehicle;
    [SerializeField] private Vector3 maxAimOffset;


    private List<Transform> playersTargets = new List<Transform>();
    private WeaponBase _weaponBase;
    
    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsServer)
        {
            playersTargets = new List<Transform>();
            _weaponBase = GetComponent<WeaponBase>();
            StartCoroutine(RefreshPlayerList());
        }
    }

    void Update()
    {
        if (Runner && Runner.IsServer)
        {
            Vector3 targetPosition = Vector3.zero;

            if (prioritizeVehicle)
            {
                targetPosition += VehicleTargets.position;
            }
            else
            {
                targetPosition += GetNearestPlayerPosition();
            }

            float distanceWithTarget = (targetPosition - transform.position).sqrMagnitude;
            if (distanceWithTarget <= lockingDistance)
            {
                targetPosition += (new Vector3(Random.Range(-maxAimOffset.x, maxAimOffset.x),
                        Random.Range(-maxAimOffset.y, maxAimOffset.y), Random.Range(-maxAimOffset.z, maxAimOffset.z)) *
                    distanceWithTarget / lockingDistance);
                transform.LookAt(targetPosition);
                _weaponBase.Shoot();
            }
        }
    }

    Vector3 GetNearestPlayerPosition()
    {
        if (playersTargets.Count <= 0) return Vector3.zero;
        
        Transform nearestPlayer = playersTargets[0];

        float currentMinDistance = (nearestPlayer.position - transform.position).sqrMagnitude;
        foreach (Transform playerPosition in playersTargets)
        {
            float testingDistance = (transform.position - playerPosition.position).sqrMagnitude;
            if (testingDistance < currentMinDistance)
            {
                nearestPlayer = playerPosition;
                currentMinDistance = testingDistance;
            }
        }

        return nearestPlayer.position;
    }
    
    
    /*
     * Refresh for connection / disconnection during the game
     */
    IEnumerator RefreshPlayerList()
    {
        WaitForSecondsRealtime waiter = new WaitForSecondsRealtime(3);
        while (true)
        {
            playersTargets.Clear();
            if (App.Instance.PlayerRefs.Count > 0)
            {
                foreach (PlayerRef player in App.Instance.PlayerRefs)
                {
                    playersTargets.Add(
                        Runner.GetPlayerObject(player)
                            .transform);
                }
            }

            yield return waiter;
        }
    }
}
