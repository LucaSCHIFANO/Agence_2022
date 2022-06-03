using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{

    public Vector2 movementInput;
    public float rotationXInput;
    public float rotationYInput;
    public NetworkBool isJumpPressed;
    public NetworkBool isRequestingToSpawn;

}
