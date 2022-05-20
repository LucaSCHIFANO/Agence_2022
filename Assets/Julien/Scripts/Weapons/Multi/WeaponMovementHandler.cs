using Fusion;
using UnityEngine;

public class WeaponMovementHandler : NetworkBehaviour
{
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            Debug.Log("Input");
            Debug.Log(input);
            Transform visual = transform.GetChild(0);
            visual.rotation = Quaternion.Euler(new Vector3(visual.rotation.eulerAngles.x + input.rotationYInput, visual.rotation.eulerAngles.y + input.rotationXInput, 0f));
        }
    }
}
