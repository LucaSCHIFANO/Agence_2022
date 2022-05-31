using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    [SerializeField] NetworkRunner networkRunnerPrefab;


    private NetworkRunner _networkRunner;
    
    // Start is called before the first frame update
    void Start()
    {
        _networkRunner = Instantiate(networkRunnerPrefab);
        _networkRunner.name = "Network Runner";

        var clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(),
            SceneManager.GetActiveScene().buildIndex, null);
        
        Debug.Log($"Server NetworkRunner Started");
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneObjectProvider = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneObjectProvider>().FirstOrDefault();

        if (sceneObjectProvider == null)
        {
            sceneObjectProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                Address = address,
                Scene = scene,
                SessionName = "TestRoom",
                Initialized = initialized,
                SceneObjectProvider = sceneObjectProvider
            }
            );
    }
    
}
