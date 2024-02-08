using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ChatRoom : NetworkBehaviour
{
    
    public static ChatRoom Instance { get; private set; }

    public event EventHandler<string> OnAddMessageText;
    public event EventHandler<List<FixedString64Bytes>> OnPlayerListUpdated;
    public event EventHandler OnChatRoomShow;

    public NetworkList<FixedString64Bytes> PlayersNames;

    private void Awake() {
        Instance = this;
        PlayersNames = new NetworkList<FixedString64Bytes>();
        //PlayersNames.OnListChanged += PlayerListUpdated;
        
    }
    

    private void Start()
    {
        //PopoteNetPart.Instance.OnDisconnect += PopoteNetPart_OnDisconnect;

        PopoteNetPart.Instance.OnPlayerDataListChange += PlayerDataListChange;
    }

    private void PlayerDataListChange(object sender, NetworkList<PlayerData> e)
    {
        OnPlayerListUpdated?.Invoke(this , PlayerData.GetListOfPlayerNamesFromPlayerDataNetWorkList(e));
    }


    private void PopoteNetPart_OnDisconnect(object sender, EventArgs e) {
        if (PlayersNames.Contains(PlayerPrefs.GetString(MainMenuUI.PLAYER_PREFS_PLAYER_NAME_MULTIPLATER))) {
            PlayersNames.Remove(PlayerPrefs.GetString(MainMenuUI.PLAYER_PREFS_PLAYER_NAME_MULTIPLATER));
        }
    }

    //private void PlayerListUpdated(NetworkListEvent<FixedString64Bytes> changeevent)
    //{
    //    OnPlayerListUpdated.Invoke(this , PlayersNames);
    //}

    //private void PlayerListUpdated(NetworkList<FixedString64Bytes> previousvalue, NetworkList<FixedString64Bytes> newvalue) {
    //    
    //}


    //public override void OnNetworkSpawn() {
    //    base.OnNetworkSpawn();
    //    Debug.Log("NetWork Spawned !!!!");
    //    OnChatRoomShow?.Invoke(this , EventArgs.Empty);
    //    
    //    AddPlayerNameServerRpc(PlayerPrefs.GetString(MainMenuUI.PLAYER_PREFS_PLAYER_NAME_MULTIPLATER));
    //    //OnAddPlayerName?.Invoke(this , PlayerPrefs.GetString(MainMenuUI.PLAYER_PREFS_PLAYER_NAME_MULTIPLATER));
    //}

   
    
    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerNameServerRpc(FixedString64Bytes name) {
        PlayersNames.Add(name);
    }
    
    
     [ServerRpc(RequireOwnership = false)]
     public void SendMessageServerRpc(FixedString64Bytes message) {
         Debug.Log(" Server Rpc Message send");
            AddTextChatClientRpc(message);
     }

     [ClientRpc]
     private void AddTextChatClientRpc(FixedString64Bytes message)
     {
         Debug.Log(" Client Rpc Message send");
         OnAddMessageText?.Invoke(this , message.ToString());
     }

     
     public void Disconnect() {
        DisconnectServerRpc();
     }

     [ServerRpc(RequireOwnership = false)]
     private void DisconnectServerRpc(ServerRpcParams serverRpcParams = default) {
         NetworkManager.Singleton.DisconnectClient(serverRpcParams.Receive.SenderClientId);
     }
}
