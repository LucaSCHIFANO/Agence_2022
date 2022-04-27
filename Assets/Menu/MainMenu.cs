using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : NetworkBehaviour
{
    public List<GameObject> GOs = new List<GameObject>();

    public List<GameObject> lobbySwitch = new List<GameObject>();
    public List<Button> lobbyButton = new List<Button>();
    public List<Slider> sliderList = new List<Slider>();

    public AudioMixer audioMix;

    [Header("Lobby List Config")]
    [SerializeField] private GameObject noLobbyFound;
    [SerializeField] private GameObject lobbyRow;
    [SerializeField] private Transform lobbyListTR;
    [SerializeField] private TMP_InputField _LobbyListCode;

    [Header("Create Lobby Config")]
    [SerializeField] private TMP_InputField _CreateLobbyName;
    [SerializeField] private Toggle _CreateLobbyIsPrivate;

    [Header("In Lobby Config")]
    [SerializeField] private Transform _InLobbyList;
    [SerializeField] private TextMeshProUGUI _InLobbyCode;
    [SerializeField] private TextMeshProUGUI _InLobbyName;

    private NetworkList<LobbyPlayersState> lobbyPlayers;

    private void Start()
    {
        lobbyPlayers = new NetworkList<LobbyPlayersState>();
        EventSystem.OnJoinLobbyEvent += OnJoinLobbyEvent;
        getSlider();
    }

    public void Play()
    {
        GOs[0].SetActive(false);
        GOs[1].SetActive(true);
    }

    public void goTOGO(int lint) //change "scene" 
    {
        for (int i = 0; i < GOs.Count; i++)
        {
            if (lint == i) GOs[i].SetActive(true);
            else GOs[i].SetActive(false);
        }
    }

    public void switchLobby(int lint) // switch join / create
    {
        for (int i = 0; i < lobbySwitch.Count; i++)
        {
            if (lint == i)
            {
                lobbySwitch[i].SetActive(true);
                lobbyButton[i].image.color = new Color(0, 0, 0, 0.7019608f);
            }
            else
            {
                lobbySwitch[i].SetActive(false);
                lobbyButton[i].image.color = new Color(0, 0, 0, 0.4f);
            }
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetSlider1(float volume)
    {
        audioMix.SetFloat("MasterVol", volume);
        saveSlider("MasterVol", volume);
    }

    public void SetSlider2(float volume)
    {
        audioMix.SetFloat("MusicVol", volume);
        saveSlider("MusicVol", volume);
    }

    public void SetSlider3(float volume)
    {
        audioMix.SetFloat("SFXVol", volume);
        saveSlider("SFXVol", volume);
    }

    private void saveSlider(string nameVol, float volume)
    {
        PlayerPrefs.SetFloat(nameVol, volume);
    }

    private void getSlider()
    {
        sliderList[0].value = PlayerPrefs.GetFloat("MasterVol");
        sliderList[1].value = PlayerPrefs.GetFloat("MusicVol");
        sliderList[2].value = PlayerPrefs.GetFloat("SFXVol");
    }


    #region Online

    async Task DisplayLobbyList()
    {
        QueryResponse lobbiesList = await LobbyManager.instance.GetLobbiesList(new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    value: "0",
                    op: QueryFilter.OpOptions.GT),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    value: "false",
                    op: QueryFilter.OpOptions.EQ)
            },
            new List<QueryOrder>() { new QueryOrder(false, QueryOrder.FieldOptions.AvailableSlots) });

        if (lobbiesList != null && lobbiesList.Results.Count >= 0)
        {
            if (lobbiesList.Results.Count <= 0)
            {
                noLobbyFound.SetActive(true);
                lobbyListTR.gameObject.SetActive(false);
                Debug.Log("No Lobby Found !");
            }
            else
            {
                noLobbyFound.SetActive(false);
                lobbyListTR.gameObject.SetActive(true);

                for (int i = 0; i < lobbyListTR.childCount; i++)
                {
                    Destroy(lobbyListTR.GetChild(i).gameObject);
                }

                Debug.Log($"We found {lobbiesList.Results.Count} lobbies");
                for (int i = 0; i < lobbiesList.Results.Count; i++)
                {
                    GameObject lobbyRowGameObject = Instantiate(lobbyRow, lobbyListTR);
                    lobbyRowGameObject.GetComponent<LobbyRowInfo>().SetLobbyInfo(lobbiesList.Results[i]);
                }
            }
        }
        else
        {
            Debug.LogError("Cannot retrieves lobby list");
        }
    }

    public async void Refresh()
    {
        await DisplayLobbyList();
    }

    public async void Join()
    {
        if (string.IsNullOrEmpty(_LobbyListCode.text)) return;

        Lobby lobby = await LobbyManager.instance.JoinLobby(lobbyCode: _LobbyListCode.text);
        if (lobby != null)
        {
            Debug.Log("Joining Lobby...");
            EventSystem.JoinLobbyEvent(lobby);
        }
    }

    public async void Create()
    {
        // Application.OpenURL("https://www.youtube.com/watch?v=dpbnVJDip6I");
        if (string.IsNullOrEmpty(_CreateLobbyName.text)) return;

        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = _CreateLobbyIsPrivate.isOn;
        Lobby lobby = await LobbyManager.instance.CreateLobby(_CreateLobbyName.text, 3, options);

        Debug.Log("Creating Lobby Completed...");
        Debug.Log("Joining Lobby...");
        EventSystem.JoinLobbyEvent(lobby);
    }

    public override void OnNetworkSpawn()
    {
        DisplayLobbyInfo(false);

        Debug.Log("Is CLient" + IsClient);
        Debug.Log("Is Server" + IsServer);

        if (IsClient)
        {
            lobbyPlayers.OnListChanged += HandleLobbyPlayersChange;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

            foreach (NetworkClient _networkClient in NetworkManager.Singleton.ConnectedClientsList)
            {
                OnClientConnectedCallback(_networkClient.ClientId);
            }
        }
    }

    public override void OnDestroy()
    {
        EventSystem.OnJoinLobbyEvent -= OnJoinLobbyEvent;
        lobbyPlayers.OnListChanged -= HandleLobbyPlayersChange;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }

        base.OnDestroy();
    }

    public async void LeaveLobby()
    {
        await LobbyManager.instance.LeaveLobby();
    }

    private void OnJoinLobbyEvent(Lobby lobbyJoined)
    {
        goTOGO(2);
        Debug.Log("Lobby Joined");
    }

    async void DisplayLobbyInfo(bool refresh)
    {
        if (!IsOwner) return;

        Debug.Log("Displaying Lobby Info");

        if (refresh) await LobbyManager.instance.RefreshCurrentLobby();

        Lobby lobbyJoined = LobbyManager.instance.GetCurrentLobby();

        if (lobbyJoined == null) return;

        _InLobbyCode.text = $"Lobby Code : {lobbyJoined.LobbyCode}";
        _InLobbyName.text = lobbyJoined.Name;

        for (int i = 0; i < lobbyJoined.MaxPlayers; i++)
        {
            _InLobbyList.GetChild(i).gameObject.SetActive(i < lobbyJoined.Players.Count);
        }

        Debug.Log("Lobby Info Completed !");
    }

    void HandleLobbyPlayersChange(NetworkListEvent<LobbyPlayersState> lobbyState)
    {
        for (int i = 0; i < _InLobbyList.childCount; i++)
        {
            _InLobbyList.GetChild(i).gameObject.SetActive(lobbyPlayers.Count > i);
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        lobbyPlayers.Add(new LobbyPlayersState()
            { ClientId = clientId, IsReady = false, PlayerName = "Aoi Bebou *w*" });
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == clientId)
            {
                lobbyPlayers.RemoveAt(i);
                break;
            }
        }
    }

    #endregion
}