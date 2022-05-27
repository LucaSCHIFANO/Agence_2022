using Fusion;
using UnityEngine;

public class WeaponMovementHandler : NetworkBehaviour
{
    private WeaponBase _weaponBase;

    public override void Spawned()
    {
        _weaponBase = GetComponent<WeaponBase>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out WeaponInputData input))
        {
            Transform visual = transform.GetChild(0);
            visual.rotation = Quaternion.Euler(new Vector3(visual.rotation.eulerAngles.x + input.rotationYInput, visual.rotation.eulerAngles.y + input.rotationXInput, 0f));

            if (input.isShooting)
            {
                _weaponBase.Shoot();
            }

            if (input.isExiting)
            {
                GetComponent<PossessableWeapon>().TryUnpossess();
            }
        }
    }
}
