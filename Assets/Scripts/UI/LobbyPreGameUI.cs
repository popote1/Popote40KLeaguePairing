using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPreGameUI : MonoBehaviour
{

    public event EventHandler<SOTeam> OnTeamSelection;
    public event EventHandler<bool> OnSetPlayerReady;
    public event EventHandler OnLeaveRoom;
    public event EventHandler OnOpenTeamSelection;
    public event EventHandler OnKickPlayer;
    public event EventHandler OnStartGame;

    [SerializeField] private TeamSelectionUI _teamSelectionUI;
    [Space(5)]
    [SerializeField] private Button _bpReturn;
    [SerializeField] private Button _bpReady;
    [SerializeField] private Button _bpSelectedTeam;
    [SerializeField] private TMP_Text _txtPlayerName1;
    [SerializeField] private TMP_Text _txtTeamName1;
    [SerializeField] private TMP_Text _txtTeamComposition1;
    [SerializeField] private TMP_Text _txtReadyButton;
    [Space(5)]
    [SerializeField] private TMP_Text _txtPlayerName2;
    [SerializeField] private TMP_Text _txtTeamName2;
    [SerializeField] private TMP_Text _txtTeamComposition2;
    [SerializeField] private TMP_Text _txtReady;
    [SerializeField] private Button _bpKick;
    [SerializeField] private Button _bpStart;

    private bool _isReady;

    public void SetSecondPlayerReady(bool value)=> _txtReady.enabled = value;
    public void SetPlayer1Name(string name) => _txtPlayerName1.text = name;
    public void SetPlayer2Name(string name) => _txtPlayerName2.text = name;
    public void SetButtonStartGame(bool value) => _bpStart.gameObject.SetActive(value);
    
    public void Hide() => gameObject.SetActive(false);
    public void EnableKickButton(bool value) => _bpKick.gameObject.SetActive(value);
    private void ClickButtonSelectTeam() => OnOpenTeamSelection?.Invoke(this , EventArgs.Empty);
    
    public void Awake() {
        _teamSelectionUI.OnConfirmSelectTeam += TeamSelectionUI_OnConfirmSelection;
        _bpSelectedTeam.onClick.AddListener(ClickButtonSelectTeam);
        _bpReturn.onClick.AddListener(()=>OnLeaveRoom?.Invoke(this , EventArgs.Empty));
        _bpReady.onClick.AddListener(ClickButtonReady);
        _bpReady.interactable = false;
        _bpKick.onClick.AddListener(()=>OnKickPlayer?.Invoke(this , EventArgs.Empty));
        _bpStart.onClick.AddListener(()=>OnStartGame?.Invoke(this , EventArgs.Empty));
        Hide();
    }

    public void Show() {
        if( !gameObject.activeSelf) ClearDisplay();
        gameObject.SetActive(true);
    }

    private void TeamSelectionUI_OnConfirmSelection(object sender, SOTeam team) {
        DisplayTeams1(team);
        OnTeamSelection?.Invoke(this , team);
        _bpReady.interactable = true;
        _teamSelectionUI.Hide();
    }
    
    public void OpenTeamSelection(List<SOTeam> teams) {
        _teamSelectionUI.Show();
        _teamSelectionUI.SetTeams(teams);
    }

    private void ClickButtonReady() {
        _isReady = !_isReady;
        OnSetPlayerReady?.Invoke(this, _isReady);
        if (_isReady) {
            _txtReadyButton.fontStyle = FontStyles.Bold;
            _txtReadyButton.color = Color.green;
        }
        else {
            _txtReadyButton.fontStyle = FontStyles.Normal;
            _txtReadyButton.color = Color.black;
        }
    }

    private void DisplayTeams1(SOTeam team) {
        if (team == null) {
            _txtTeamName1.text = "";
            _txtTeamComposition1.text = "";
                return;
        }
        _txtTeamName1.text = team.TeamName;
        _txtTeamComposition1.text = team.GetDescription();
    }

    private void ClearDisplay() {
        _txtPlayerName1.text = "";
        _txtTeamComposition1.text = "";
        _txtTeamName1.text = "";

        ClearOpponentDisplay();

        _bpKick.gameObject.SetActive(false);
        _txtReady.enabled =false;
    }

    private void ClearOpponentDisplay() {
        _txtPlayerName2.text = "";
        _txtTeamComposition2.text = "";
        _txtTeamName2.text = "";
    }
    
    public void DisplayTeams2(SOTeam team) {
        if (team == null) {
            _txtTeamName2.text = "";
            _txtTeamComposition2.text = "";
            return;
        }
        _txtTeamName2.text = team.TeamName;
        _txtTeamComposition2.text = team.GetDescription();
        Debug.Log("Team Display contain "+ team.Factions.Count+ " Factions");
    }
    
    

    
}
