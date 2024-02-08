using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class LobbyPreGameLogic :MonoBehaviour
{
    
    [SerializeField] private LobbyPreGameUI _lobbyPreGameUI;
    

    [SerializeField] private List<SOCarte> _cartes;

    private List<SOTeam> _teams;
    private ulong _currentID;
    private PlayerData _localPlayerData;
    private PlayerData _opponent;
    private SOTeam _selectedTeam;
    private SOTeam _oppenentTeam;
    
    private void LobbyPreGameUI_OnOpenTeamSelection(object sender, EventArgs e) => LoadTeams();

    private void Awake() {
        _lobbyPreGameUI.OnOpenTeamSelection += LobbyPreGameUI_OnOpenTeamSelection;
        _lobbyPreGameUI.OnSetPlayerReady += LobbyPreGameUI_OnSetPlayerReady;
        _lobbyPreGameUI.OnLeaveRoom += LobbyPreGameUI_OnLeaveRoom;
        _lobbyPreGameUI.OnKickPlayer += LobbyPreGameUI_OnKickPlayer;
        _lobbyPreGameUI.OnTeamSelection += LobbyPreGameLogic_OnTeamSelection;
        _lobbyPreGameUI.OnStartGame += LobbyPreGameLogic_OnStartGame;
    }

    
    private void Start() {
        PopoteNetPart.Instance.OnPlayerDataListChange += PopoteNetPart_OnPlayerListChange;
        PopoteNetPart.Instance.OnConnectionDone += PopoteNetPart_OnConnectionDone;
        PopoteNetPart.Instance.OnDisconnect += PopoteNetPart_OnClientDisconnect;
        PopoteNetPart.Instance.OnTeamSubmite += PopoteNetPart_OnTeamSubmit;
        PopoteNetPart.Instance.OnStartPairing += PopoteNetPart_OnPairingStart;
    }

    private void PopoteNetPart_OnPairingStart(object sender, EventArgs e)
    {
        _lobbyPreGameUI.Hide();
    }

    private void LobbyPreGameLogic_OnStartGame(object sender, EventArgs e) {
        PopoteNetPart.Instance.StartPairingServerRPC(
            _localPlayerData, _selectedTeam.CreateSerializationData(),
            _opponent, _oppenentTeam.CreateSerializationData());
    }

    private void PopoteNetPart_OnTeamSubmit(object sender, SerializeSOTeam eSerializeSoTeam) {
        SOTeam team = ScriptableObject.CreateInstance<SOTeam>();
        team.LoadDataFromSerializeData(eSerializeSoTeam);
        _oppenentTeam = team;
        _lobbyPreGameUI.DisplayTeams2(team);
    }

    private void LobbyPreGameLogic_OnTeamSelection(object sender, SOTeam team) {
        SerializeSOTeam serializeSoTeam = team.CreateSerializationData();
        PopoteNetPart.Instance.SubmitTeam(serializeSoTeam);
        _selectedTeam = team;
    }

    private void PopoteNetPart_OnClientDisconnect(object sender, ulong id) { 
        _lobbyPreGameUI.Hide();
        _selectedTeam = null;
    }

    private void LobbyPreGameUI_OnKickPlayer(object sender, EventArgs e) {
        if( !String.IsNullOrEmpty(_opponent.PlayerName.ToString())){
            PopoteNetPart.Instance.KickPlayer(_opponent.ClientId);
        }
    }

    private void LobbyPreGameUI_OnLeaveRoom(object sender, EventArgs e) {
        PopoteNetPart.Instance.SelfDisconnectServerRPC();
        _lobbyPreGameUI.Hide();
    }

    private void LobbyPreGameUI_OnSetPlayerReady(object sender, bool value) {
        PopoteNetPart.Instance.SetPlayerReadyServerRPC(value);
    }
   

    private void PopoteNetPart_OnConnectionDone(object sender, EventArgs e) {
        _lobbyPreGameUI.Show();
        _currentID = PopoteNetPart.Instance.GetCurrentNetworkId();
        DisplayerPlayerdata(PopoteNetPart.Instance.GetPlayerDataList);
        
    }
    
    private void PopoteNetPart_OnPlayerListChange(object sender, NetworkList<PlayerData> playersData) {
        DisplayerPlayerdata(playersData);
        
        //if(_selectedTeam!=null&&!String.IsNullOrEmpty(_selectedTeam.TeamName))PopoteNetPart.Instance.SubmitTeam(_selectedTeam.CreateSerializationData());
    }

    private void DisplayerPlayerdata( NetworkList<PlayerData> playersData)
    {
        //Debug.Log("Displaying player Data =>"+playersData.Count);
        PlayerData localPlayerData = new PlayerData();
        PlayerData oponentPlayerData = new PlayerData();
        foreach (var data in playersData) {
            if (data.ClientId == PopoteNetPart.Instance.GetCurrentNetworkId()) {
                localPlayerData = data;
                _localPlayerData = data;
            }
            else {
                oponentPlayerData = data;
                _opponent = data;
            }
        }
        _opponent = oponentPlayerData;
       // Debug.Log(" Player 1 ="+ localPlayerData.PlayerName + "    Player2 ="+ OponentPlayerData.PlayerName +"        local playerName ="+ MainMenuUI.PlayerName);
        _lobbyPreGameUI.SetPlayer1Name(localPlayerData.PlayerName.ToString());
        _lobbyPreGameUI.SetPlayer2Name(oponentPlayerData.PlayerName.ToString());
        _lobbyPreGameUI.SetSecondPlayerReady(oponentPlayerData.IsReady);
        _lobbyPreGameUI.EnableKickButton(!String.IsNullOrEmpty(oponentPlayerData.PlayerName.ToString()) && localPlayerData.ClientId == 0) ;
        if (String.IsNullOrEmpty(oponentPlayerData.PlayerName.ToString())) _lobbyPreGameUI.DisplayTeams2(null);
        if( PopoteNetPart.Instance.IsServer)_lobbyPreGameUI.SetButtonStartGame(IsAllPlayerReady(playersData));
    }
    
    public void LoadTeams() {
        _teams = LoadSavedTeams();
        _lobbyPreGameUI.OpenTeamSelection(_teams);
    }
    
    private List<SOTeam> LoadSavedTeams() {
        List<SOTeam>savedTeam = new List<SOTeam>();
        foreach (string path in Directory.GetFiles(Application.persistentDataPath)) {
            try {
                SerializeSOTeam data = JsonUtility.FromJson<SerializeSOTeam>(File.ReadAllText(path));
                SOTeam so = ScriptableObject.CreateInstance<SOTeam>();
                so.LoadDataFromSerializeData(data);
                savedTeam.Add(so);
            }
            catch (Exception e) {
            }
        }
        return savedTeam;
    }
    private bool IsAllPlayerReady(NetworkList<PlayerData> playerData) {
        if (playerData.Count < 2) return false;
        foreach (var player in playerData)  if (!player.IsReady) return false; 
        return true;
    }
    
}