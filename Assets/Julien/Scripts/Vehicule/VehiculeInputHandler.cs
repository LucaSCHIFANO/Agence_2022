using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiculeInputHandler : MonoBehaviour
{
    Vector2 viewInputVector = Vector2.zero;
    private bool isShooting;
    private bool isExiting;
    private bool isHonking;
    private bool leftControl;
    
    private Rewired.Player player;
    private bool shift;
    private bool shiftUp;
    private bool shiftDown;
    private bool breaking;
    private Vector2 movement;
    private bool teleportToSpawn;
    private bool teleportToBigDrop;
    private bool teleportToBigDrop2;
    private bool teleportToBigDrop3;
    private bool teleportToBigDrop4;

    // Start is called before the first frame update
    void Start()
    {
        player = Rewired.ReInput.players.GetPlayer(0);
    }

    // Update is called once per frame
    void Update()
    {
        shiftUp = Input.GetKeyDown("page up");
        shiftDown = Input.GetKeyDown("page down");
        breaking = player.GetButton("Breaking");
        movement.x = player.GetAxis("Turn");
        movement.y = player.GetAxis("Throttle");
        // shift = Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift);
        isExiting = Input.GetKey(KeyCode.E);
        isHonking = Input.GetKeyDown(KeyCode.H);
        leftControl = Input.GetKey(KeyCode.LeftShift);
        teleportToSpawn = Input.GetKeyDown(KeyCode.Alpha1);
        teleportToBigDrop = Input.GetKeyDown(KeyCode.Alpha2);
        teleportToBigDrop2 = Input.GetKeyDown(KeyCode.Alpha3);
        teleportToBigDrop3 = Input.GetKeyDown(KeyCode.Alpha4);
        teleportToBigDrop4 = Input.GetKeyDown(KeyCode.Alpha5);
    }

    public VehiculeInputData GetNetworkInput()
    {
        VehiculeInputData inputData = new VehiculeInputData();

        inputData.movement = movement;
        inputData.breaking = breaking;
        inputData.shift = shift;
        inputData.shiftUp = shiftUp;
        inputData.shiftDown = shiftDown;
        inputData.isExiting = isExiting;
        inputData.isHonking = isHonking;
        inputData.leftControl = leftControl;
        inputData.teleportToSpawn = teleportToSpawn;
        
        inputData.teleportToBigDrop = teleportToBigDrop;
        inputData.teleportToBigDrop2 = teleportToBigDrop2;
        inputData.teleportToBigDrop3 = teleportToBigDrop3;
        inputData.teleportToBigDrop4 = teleportToBigDrop4;

        return inputData;
    }
}
