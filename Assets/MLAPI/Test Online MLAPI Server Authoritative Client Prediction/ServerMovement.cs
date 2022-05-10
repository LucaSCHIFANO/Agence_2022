using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Netcode;

public class ServerMovement : NetworkBehaviour
{
    private Dictionary<ulong, Queue<ClientInputState>> clientInputs = new Dictionary<ulong, Queue<ClientInputState>>();

    private void Start()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ClientInputState", OnClientInputStateReceived);
        }
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        
        foreach (KeyValuePair<ulong, Queue<ClientInputState>> entry in clientInputs)
        {
            CharacterControllerPrediction controller = NetworkManager.ConnectedClients[entry.Key].PlayerObject
                .gameObject.GetComponent<CharacterControllerPrediction>();

            Queue<ClientInputState> queue = entry.Value;

            ClientInputState inputState = null;

            while (queue.Count > 0 && (inputState = queue.Dequeue()) != null)
            {
                controller.ProcessInputs(inputState);

                SimulationState state = controller.CurrentSimulationState(inputState);
                
                using FastBufferWriter writer = new FastBufferWriter(Marshal.SizeOf(state) + 12, Allocator.Temp);
                writer.WriteNetworkSerializable(state);
        
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ServerSimulationState", entry.Key, writer);
            }
        }
    }

    private void OnClientInputStateReceived(ulong senderClientId, FastBufferReader messagePayload)
    {
        messagePayload.ReadNetworkSerializable(out ClientInputState cliState);
        if (cliState != null)
        {
            if (clientInputs.ContainsKey(senderClientId) == false)
            {
                clientInputs.Add(senderClientId, new Queue<ClientInputState>());
            }
            
            clientInputs[senderClientId].Enqueue(cliState);
        }
    }
}
