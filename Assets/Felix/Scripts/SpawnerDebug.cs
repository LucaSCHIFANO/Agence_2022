using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerDebug : MonoBehaviour
{
    private NetworkedPlayer player;
    private bool isOpen;

    [SerializeField] private GameObject nUi;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject prefabUI;
    [Space]
    [SerializeField] private GameObject[] prefabsToSpawn;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        for (int i = 0; i < prefabsToSpawn.Length; i++)
        {
            GameObject ui = Instantiate(prefabUI, parent);
            Button button = ui.GetComponent<Button>();
            button.onClick.RemoveAllListeners();

            int t = i;
            button.onClick.AddListener(() =>
            {
                SpawnGameObject(prefabsToSpawn[t]);
            });

            ui.transform.GetChild(0).GetComponent<TMP_Text>().text = prefabsToSpawn[i].name;
        }
        
        nUi.gameObject.SetActive(false);
        isOpen = false;
    }

    private void Update()
    {
        if (!isOpen && Input.GetKeyDown(KeyCode.T))
        {
            OpenPanel();
        }
    }

    public void OpenPanel()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        nUi.gameObject.SetActive(true);
        isOpen = true;

        if (player == null)
        {
            NetworkedPlayer[] players = FindObjectsOfType<NetworkedPlayer>();

            foreach (NetworkedPlayer nPlayer in players)
            {
                if (nPlayer.Object.HasInputAuthority)
                {
                    player = nPlayer;
                    break;
                }
            }
        }
        
        player.enabled = false;
    }

    public void ClosePanel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        nUi.gameObject.SetActive(false);
        isOpen = false;

        player.enabled = true;
    }

    public void SpawnGameObject(GameObject _gameObject)
    {
        NetworkObject networkObject = _gameObject.GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Instantiate(_gameObject, new Vector3(0f, 1f, 0f), Quaternion.identity);
        }
        else
        {
            player.Runner.Spawn(networkObject, new Vector3(0f, 1f, 0f));
        }
    }
}
