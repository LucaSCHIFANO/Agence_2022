using Fusion;

public struct WeaponInputData : INetworkInput
{
    public float rotationXInput;
    public float rotationYInput;
    public NetworkBool isShooting;
    public NetworkBool isExiting;
}
