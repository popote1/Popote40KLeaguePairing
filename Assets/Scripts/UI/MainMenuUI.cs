using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenuUI : MonoBehaviour {

    public const string PLAYER_PREFS_PLAYER_NAME_MULTIPLATER = "PlayerNameMultyPlayer";

    public static string PlayerName;
    
    [SerializeField] private TMP_InputField _inputFieldPlayerName;
    [SerializeField] private Button _bpNetwork;
    [SerializeField] private Button _bpLibrary;
    [SerializeField] private ConnectionUI _connectionUI;

    private string _playerName;
    
    


    private void Awake() {
        _playerName =  "Popote"+ Random.Range(100, 1000);
        PlayerName = _playerName;
        //PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLATER , _playerName);
        //_playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLATER, "PlayerName" + Random.Range(100, 1000));
        
        _inputFieldPlayerName.SetTextWithoutNotify(_playerName);
        _inputFieldPlayerName.onValueChanged.AddListener(SetPlayerName);
        _bpNetwork.onClick.AddListener(_connectionUI.Show);
        _bpLibrary.onClick.AddListener(()=>LoadLibraryScene());
    }

    public string GetPlayerName() => _playerName;
    

    public void SetPlayerName(string playerName) {
        _playerName = playerName;
        PlayerName = playerName;
        //PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLATER , playerName);
    }

    private void GoToNetWorkScene()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadLibraryScene() {
        SceneManager.LoadScene(3);
    }
    
    
}

