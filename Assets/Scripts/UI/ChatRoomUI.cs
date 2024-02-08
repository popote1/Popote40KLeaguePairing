using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoomUI : MonoBehaviour
{

    [SerializeField] private TMP_InputField _inputFieldChat;
    [SerializeField] private Transform _transformPlayerHolder;
    [SerializeField] private TMP_Text _txtTemplatePlayerName;
    [SerializeField] private Transform _transformChatHolder;
    [SerializeField] private TMP_Text _txtTemplateChatText;
    [SerializeField] private int _maxChatText = 5;

    [SerializeField] private Button _bpDisconnect;
    // Start is called before the first frame update
    
    public void Show() => gameObject.SetActive(true);
    private void Hide()=> gameObject.SetActive(false);
    
    void Start()
    {
        ChatRoom.Instance.OnAddMessageText += AddTextChat;
        ChatRoom.Instance.OnChatRoomShow += OnChatRoom_Show;
        ChatRoom.Instance.OnPlayerListUpdated += ChatRoom_OnPlayerListUpdated;
        _bpDisconnect.onClick.AddListener(()=>{
            Hide();
            ChatRoom.Instance.Disconnect();
        });
        _inputFieldChat.onSubmit.AddListener(SendMessage);
        ClearChatBox();
        ClearPlayersList();
        Hide();
    }

    private void ChatRoom_OnPlayerListUpdated(object sender, List<FixedString64Bytes> playerList)
    {
       ClearPlayersList();
       foreach (var player in playerList) {
           TMP_Text playerName = Instantiate(_txtTemplatePlayerName, _transformPlayerHolder);
           playerName.text = player.ToString();
           playerName.gameObject.SetActive(true);
       }
    }

    private void OnChatRoom_Show(object sender, EventArgs e)=> Show();
    

    private void SendMessage(string message) {
        ChatRoom.Instance.SendMessageServerRpc(message);
        _inputFieldChat.SetTextWithoutNotify(null);
    }
    

    private void AddTextChat(object sender , string text) {
        Debug.Log("UI Message Posted");
        if (_transformChatHolder.childCount >= _maxChatText) ClearChatBox();
            
        TMP_Text Message = Instantiate(_txtTemplateChatText, _transformChatHolder);
        Message.text = text.ToString();
        Message.gameObject.SetActive(true);
    }
    
    
    private void ClearPlayersList() {
        foreach (Transform player in _transformPlayerHolder) {
            if (player == _txtTemplatePlayerName.transform) {
                _txtTemplatePlayerName.gameObject.SetActive(false);
                continue;
            }
            player.gameObject.SetActive(false);
            Destroy(player.gameObject, 0.01f);
        }
    }

    private void ClearChatBox() {
        foreach (Transform chat in _transformChatHolder) {
            if (chat == _txtTemplateChatText.transform) {
                _txtTemplateChatText.gameObject.SetActive(false);
                continue;
            }
            chat.gameObject.SetActive(false);
            Destroy(chat.gameObject, 0.01f);
        }
    }
    
    
}
