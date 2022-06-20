using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class CreationLobbyPanel : MonoBehaviour
{
    [SerializeField] private InputField _inputName;
    [SerializeField] private Text _textMaxPlayers;
    [SerializeField] private Toggle _toggleMap1;
    [SerializeField] private Toggle _toggleMap2;
    [SerializeField] private int _codeLength = 6;

    private int _maxPly = 4;

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnDecreaseMaxPlayers()
    {
        if (_maxPly > 2)
            _maxPly--;
        UpdateUI();
    }

    public void OnIncreaseMaxPlayers()
    {
        if (_maxPly < 16)
            _maxPly++;
        UpdateUI();
    }

    public void OnEditText()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        _textMaxPlayers.text = $"Max Players: {_maxPly}";
        if (!_toggleMap1.isOn && !_toggleMap2.isOn)
            _toggleMap1.isOn = true;
        if (string.IsNullOrWhiteSpace(_inputName.text))
            _inputName.text = "Room1";
    }
		
    public void OnCreateSession()
    {
        SessionProps props = new SessionProps();
        props.StartMap = _toggleMap1.isOn ? MapIndex.Camion : MapIndex.Map1;
        props.PlayerLimit = _maxPly;
        props.RoomName = GenerateRandomCode();
        props.LobbyName = _inputName.text;
        props.IsPrivate = false;
        App.Instance.CreateSession(props);
    }

    private string GenerateRandomCode()
    {
        Random random = new Random();
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        return new string(Enumerable.Repeat(chars, _codeLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}