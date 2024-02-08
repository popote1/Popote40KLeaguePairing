using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PopoteNetPart : NetworkBehaviour
{
    
    public static PopoteNetPart Instance { get; private set; }

    public event EventHandler OnServerSpawn;
    public event EventHandler OnConnectionDone;
    public event EventHandler<ulong> OnDisconnect;
    public event EventHandler<NetworkList<PlayerData>> OnPlayerDataListChange;
    public event EventHandler<SerializeSOTeam> OnTeamSubmite;
    public event EventHandler OnStartPairing;
    

    public ulong LocalID;

    [SerializeField] private PairingLogic _pairingLogic;
    
    private NetworkList<PlayerData> _playerDataList;
    private Dictionary<ulong, SerializeSOTeam> _playersTeams;

    public NetworkList<PlayerData> GetPlayerDataList => _playerDataList;
    public ulong GetCurrentNetworkId() => NetworkManager.LocalClientId;
    public bool IsHost() => IsServer;
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _playerDataList = new NetworkList<PlayerData>();
        _playerDataList.OnListChanged += PlayerDataList_OnListChange;
        _playersTeams = new Dictionary<ulong, SerializeSOTeam>();
    }

    private void PlayerDataList_OnListChange(NetworkListEvent<PlayerData> changeEvent) {
        OnPlayerDataListChange?.Invoke(this , _playerDataList);
    }

    public void StartHost() {

        Debug.Log("Try Starting Host");
        NetworkManager.Singleton.OnClientConnectedCallback+= NetworkManager_Server_OnOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallBack;
        NetworkManager.Singleton.StartHost();
        Debug.Log("Start Host");
    }

    private void NetworkManager_Server_OnClientDisconnectCallBack(ulong clientId) {
        for (int i = 0; i < _playerDataList.Count; i++) {
            PlayerData player = _playerDataList[i];
            if (player.ClientId == clientId) {
                _playerDataList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_Server_OnOnClientConnectedCallback(ulong clientId) {
       _playerDataList.Add( new PlayerData() {
           ClientId = clientId
       });
       Debug.Log("Do That");
       OnConnectionDone?.Invoke(this ,EventArgs.Empty);
       SetPlayerNameServerRPC(MainMenuUI.PlayerName);
       if (_playersTeams.Keys.Contains((ulong)0)) {
           SubmitTeamClientRpc(_playersTeams[(ulong)0], 0);
       }
       //SetPlayerNameServerRPC(PlayerPrefs.GetString(MainMenuUI.PLAYER_PREFS_PLAYER_NAME_MULTIPLATER));
    }

    public void StartClient() {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallBack;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallBack;
        NetworkManager.Singleton.StartClient();
        Debug.Log("Start Client");
    }

    private void NetworkManager_Server_OnClientConnectedCallBack(ulong id) {
        
        OnConnectionDone?.Invoke(this ,EventArgs.Empty);
        SetPlayerNameServerRPC(MainMenuUI.PlayerName);
        //SetPlayerNameServerRPC(PlayerPrefs.GetString(MainMenuUI.PLAYER_PREFS_PLAYER_NAME_MULTIPLATER));
        if( IsLocalPlayer)LocalID = id;
        
    }

    private void NetworkManager_Client_OnClientDisconnectCallBack(ulong id) {
       OnDisconnect?.Invoke(this, id);
        
    }

    public void SubmitTeam(SerializeSOTeam team) {
        SubmitTeamServerRPC(team);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitTeamServerRPC(SerializeSOTeam team ,ServerRpcParams serverRpcParams = default)
    {
        if (_playersTeams.Keys.Contains(serverRpcParams.Receive.SenderClientId)) _playersTeams[serverRpcParams.Receive.SenderClientId] = team;
        else _playersTeams.Add(serverRpcParams.Receive.SenderClientId ,team);
        
        SubmitTeamClientRpc(team, serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SubmitTeamClientRpc(SerializeSOTeam team , ulong playerId) {
        if (playerId == GetCurrentNetworkId()) return;
        OnTeamSubmite?.Invoke(this  ,team);
    }
    
    public override void OnNetworkSpawn() {
        OnServerSpawn?.Invoke(this , EventArgs.Empty);
        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRPC(string name, ServerRpcParams serverRpcParams = default) {
        int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataList[playerIndex];

        playerData.PlayerName = name;
        _playerDataList[playerIndex] = playerData;
    }

    [ServerRpc(RequireOwnership =  false)]
    public void SetPlayerReadyServerRPC(bool value, ServerRpcParams serverRpcParams = default)
    {
        int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataList[playerIndex];
        playerData.IsReady = value;
        _playerDataList[playerIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelfDisconnectServerRPC(ServerRpcParams serverRpcParams = default) {
        NetworkManager.Singleton.DisconnectClient(serverRpcParams.Receive.SenderClientId);
        if (serverRpcParams.Receive.SenderClientId == 0) {
            NetworkManager.Shutdown();
        }
    }

    
    public void KickPlayer(ulong id) {
        if (!IsServer) return;
        if (!NetworkManager.Singleton.ConnectedClientsIds.Contains(id)) {
            Debug.LogWarning("Id Not Connected");
            return;
        }
        NetworkManager.DisconnectClient(id);
    }
    
    private int GetPlayerDataIndexFromClientId(ulong ClientId) {
        for (int i = 0; i < _playerDataList.Count; i++) {
            if (_playerDataList[i].ClientId == ClientId) {
                return i;
            }
        }
        return -1;
    }

    [ServerRpc]
    public void StartPairingServerRPC(PlayerData player1,SerializeSOTeam team1 ,PlayerData player2,SerializeSOTeam team2 ) {
        StartPairingClientRPC(player1, team1, player2, team2);
    }

    [ClientRpc]
    public void StartPairingClientRPC(PlayerData player1, SerializeSOTeam team1, PlayerData player2, SerializeSOTeam team2) {
        _pairingLogic.SetupPairing(player1, team1, player2, team2);
        OnStartPairing?.Invoke(this , EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)] public void PairingSingleSubmitServerRpc(int id, ServerRpcParams serverRpcParams = default) {
        _pairingLogic.ServerSingleSubmite(id, serverRpcParams.Receive.SenderClientId);   
    }
    [ServerRpc(RequireOwnership = false)] public void PairingDoubleSubmitServerRpc(int id1,int id2 ,ServerRpcParams serverRpcParams = default) {
        _pairingLogic.ServerDoubleSubmite(id1,id2, serverRpcParams.Receive.SenderClientId);   
    }
    [ServerRpc(RequireOwnership = false)] public void ChoseSubmitServerRpc(int id1,ServerRpcParams serverRpcParams = default) {
        _pairingLogic.ServerChoseSubmite(id1, serverRpcParams.Receive.SenderClientId);   
    }

    [ServerRpc] public void EndSimpleStatServerRpc(int cartPlayer1 ,int cartPlayer2 ) {
        EndSimpleStatClientRpc(cartPlayer1, cartPlayer2);
    }
    [ClientRpc] public void EndSimpleStatClientRpc(int cartPlayer1 ,int cartPlayer2 ) {
        _pairingLogic.EndSimpleStat(cartPlayer1, cartPlayer2);
    }
    
    [ServerRpc] public void EndDoubleStatServerRpc(int cart1Player1 ,int cart2Player1,int cart1Player2,int cart2Player2 ) {
        EndSimpleStatClientRpc(cart1Player1, cart2Player1,cart1Player2,cart2Player2);
    }
    [ClientRpc] public void EndSimpleStatClientRpc(int cart1Player1 ,int cart2Player1,int cart1Player2,int cart2Player2) {
        _pairingLogic.EndDoubleStat(cart1Player1, cart2Player1,cart1Player2,cart2Player2);
    }
    
    [ServerRpc] public void EndChoseStatServerRpc(int cartPlayer1 ,int cartPlayer2) {
        EndChoseStatClientRpc(cartPlayer1, cartPlayer2);
    }
    [ClientRpc] public void EndChoseStatClientRpc(int cartPlayer1 ,int cartPlayer2) {
        _pairingLogic.EndChoseState(cartPlayer1, cartPlayer2);
    }



}