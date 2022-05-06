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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : NetworkBehaviour
{
    public List<GameObject> GOs = new List<GameObject>();

    public List<GameObject> lobbySwitch = new List<GameObject>();
    public List<Button> lobbyButton = new List<Button>();
    public List<Slider> sliderList = new List<Slider>();

    public AudioMixer audioMix;

    [Header("Options")]
    [SerializeField] private TMP_InputField _usernameField;
    [SerializeField] private List<Color> _colorSkin;
    [SerializeField] private Image _skinImage;

    [Header("Lobby List Config")]
    [SerializeField] private GameObject noLobbyFound;
    [SerializeField] private GameObject lobbyRow;
    [SerializeField] private Transform lobbyListTR;
    [SerializeField] private TMP_InputField _LobbyListCode;

    [Header("Create Lobby Config")]
    [SerializeField] private TMP_InputField _CreateLobbyName;
    [SerializeField] private Toggle _CreateLobbyIsPrivate;

    [Header("In Lobby Config")]
    [SerializeField] public Transform _InLobbyList;
    [SerializeField] private GameObject _InLobbyPlayerRow;
    [SerializeField] private TextMeshProUGUI _InLobbyCode;
    [SerializeField] private TextMeshProUGUI _InLobbyName;
    [SerializeField] private GameObject _InLobbyLaunchButton;

    [SerializeField] private string gameMapName = "CTest";

    private NetworkList<LobbyPlayersState> lobbyPlayers;

    private int currentSelectedColor;

    public static MainMenu instance;

    private void Awake()
    {
        if (instance != null) Destroy(this);
        instance = this;
    }

    private void Start()
    {
        lobbyPlayers = new NetworkList<LobbyPlayersState>();
        EventSystem.OnJoinLobbyEvent += OnJoinLobbyEvent;
        getSlider();
        GetUsername();
        GetSkinColor();
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

    public void SetUsername(string username)
    {
        PlayerPrefs.SetString("username", username);
    }

    public void GetUsername()
    {
        if (!PlayerPrefs.HasKey("username"))
        {
            PlayerPrefs.SetString("username", "u gay");
        }
        
        _usernameField.text = PlayerPrefs.GetString("username");
    }

    public void GetSkinColor()
    {
        if (!PlayerPrefs.HasKey("skinColor"))
        {
            PlayerPrefs.SetInt("skinColor", 0);
        }

        ChangeSkinColor(PlayerPrefs.GetInt("skinColor"));
    }

    public void NextSkin(int skip)
    {
        ChangeSkinColor(currentSelectedColor + skip);
    }

    private void ChangeSkinColor(int getInt)
    {
        currentSelectedColor = getInt;

        if (currentSelectedColor >= _colorSkin.Count) currentSelectedColor = 0;
        if (currentSelectedColor < 0) currentSelectedColor = (_colorSkin.Count - 1);
        
        PlayerPrefs.SetInt("skinColor", currentSelectedColor);
        _skinImage.color = _colorSkin[currentSelectedColor];
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

        Lobby lobby = await LobbyManager.instance.JoinLobby(PlayerPrefs.GetString("username"), PlayerPrefs.GetInt("skinColor"), lobbyCode: _LobbyListCode.text);
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
        Lobby lobby = await LobbyManager.instance.CreateLobby(_CreateLobbyName.text, 3, options, PlayerPrefs.GetString("username"), PlayerPrefs.GetInt("skinColor"));

        Debug.Log("Creating Lobby Completed...");
        Debug.Log("Joining Lobby...");
        EventSystem.JoinLobbyEvent(lobby);
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyPlayers.OnListChanged += HandleLobbyPlayersChange;

            DisplayLobbyInfo(false);
        }

        if (IsServer)
        {
            // _InLobbyList.GetComponent<NetworkObject>().Spawn();
            
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
        base.OnDestroy();
        
        EventSystem.OnJoinLobbyEvent -= OnJoinLobbyEvent;
        lobbyPlayers.OnListChanged -= HandleLobbyPlayersChange;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
    }

    public async void LeaveLobby()
    {
        await LobbyManager.instance.LeaveLobby();
    }

    private void OnJoinLobbyEvent(Lobby lobbyJoined)
    {
        goTOGO(2);
        Debug.Log("Lobby Joined");
        DisplayLobbyInfo(false);
    }

    async void DisplayLobbyInfo(bool refresh)
    {
        Debug.Log("Displaying Lobby Info");

        if (refresh) await LobbyManager.instance.RefreshCurrentLobby();

        Lobby lobbyJoined = LobbyManager.instance.GetCurrentLobby();

        if (lobbyJoined == null) return;

        _InLobbyCode.text = $"Lobby Code : {lobbyJoined.LobbyCode}";
        _InLobbyName.text = lobbyJoined.Name;

        for (int i = 0; i < _InLobbyList.childCount; i++)
        {
            if (i < lobbyPlayers.Count)
            {
                _InLobbyList.GetChild(i).gameObject.SetActive(true);
                _InLobbyList.GetChild(i).GetComponent<PlayerRowDisplay>().DisplayPlayerName(lobbyPlayers[i].PlayerName);
            }
            else
            {
                _InLobbyList.GetChild(i).gameObject.SetActive(false);
            }
        }

        if (IsHost)
        {
            _InLobbyLaunchButton.SetActive(true);
        }

        Debug.Log("Lobby Info Completed !");
    }
    

    private void OnClientConnectedCallback(ulong clientId)
    {
        lobbyPlayers.Add(new LobbyPlayersState()
            { ClientId = clientId, IsReady = false, PlayerName = RelayManager.GetPlayerData(clientId).HasValue ? RelayManager.GetPlayerData(clientId).Value.Username : "Player X" });
        
        // GameObject playerRow = Instantiate(_InLobbyPlayerRow);
        // playerRow.GetComponent<NetworkObject>().Spawn(true);
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == clientId)
            {
                lobbyPlayers.RemoveAt(i);
                // Destroy(_InLobbyList.GetChild(i).gameObject);
                break;
            }
        }
    }
    
    void HandleLobbyPlayersChange(NetworkListEvent<LobbyPlayersState> lobbyState)
    {
        for (int i = 0; i < _InLobbyList.childCount; i++)
        {
            if (i < lobbyPlayers.Count)
            {
                _InLobbyList.GetChild(i).gameObject.SetActive(true);
                _InLobbyList.GetChild(i).GetComponent<PlayerRowDisplay>().DisplayPlayerName(lobbyPlayers[i].PlayerName);
            }
            else
            {
                _InLobbyList.GetChild(i).gameObject.SetActive(false);
            }
        }
        
        // DisplayLobbyInfo(false);
    }


    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(gameMapName, LoadSceneMode.Single);
    }

    #endregion
}