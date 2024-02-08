using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamSelectorLibrary : MonoBehaviour
{
    [SerializeField] private LibraryLogic _libraryLogic;
    [SerializeField] private Transform _transformTeamHolder;
    [SerializeField] private Button _bpCreateNewTeam;
    [SerializeField] private Button _bpMainMenu;
    [SerializeField] private Button _bpDelete;
    [SerializeField] private Button _bpRefresh;
    [SerializeField] private TeamSelectionButton _prefabTeamSelectionButton;
    [SerializeField] private TMP_Text _txtTeamName;
    [SerializeField] private TMP_Text _txtDescription;
    [SerializeField] private Button _bpCutom;

    public event EventHandler OnButtonCreateNewTeamClicked;
    
    private void Awake() {
        
        _libraryLogic.OnCloseTeamBuilding += LibraryLogic_OnCloseTeamBuilding;
        _libraryLogic.OnSavedTeamUpdate += LibraryLogic_OnSavedTeamUpdate;
        _libraryLogic.OnOpenTeamBuilding += LibraryLogic_OpenTeamBuilding;
        _libraryLogic.OnSelectTeam += LibraryLogic_OnSelectTeam;
        
        _bpCreateNewTeam.onClick.AddListener (()=> OnButtonCreateNewTeamClicked?.Invoke(this , EventArgs.Empty));
        _bpMainMenu.onClick.AddListener(()=> ReturnToMainMenu());
        _bpCutom.onClick.AddListener(()=>CuromizTeam());
        _bpDelete.onClick.AddListener(()=>DeleteSelectedTeam());
        _bpRefresh.onClick.AddListener(()=>RefreshTeams());

    }

    private void RefreshTeams() => _libraryLogic.UpdateSavedTeams();
    private void DeleteSelectedTeam() => _libraryLogic.DeleteSelectedTeam();

    private void Start() {
        _libraryLogic.UpdateSavedTeams();
    }

    private void LibraryLogic_OnSelectTeam(object sender, SOTeam team)=> DisplayTeams(team);
    private void LibraryLogic_OpenTeamBuilding(object sender, SOTeam e) => Hide();
    private void LibraryLogic_OnCloseTeamBuilding(object sender, EventArgs e)=> Show();
    private void LibraryLogic_OnSavedTeamUpdate(object sender, List<SOTeam> teams) => SetTeams(teams);
    private void ReturnToMainMenu() =>SceneManager.LoadScene(0);
    private void CuromizTeam() => _libraryLogic.CutomizTeam();
    private void ClearTeamHolder() { 
        for (int i = 0; i < _transformTeamHolder.childCount; i++) {
            _transformTeamHolder.GetChild(i).gameObject.SetActive(false);
            if (_transformTeamHolder.GetChild(i) == _prefabTeamSelectionButton.transform) continue;
            Destroy(_transformTeamHolder.GetChild(i).gameObject, 0.1f);
            
        }
    }

    public void SetTeams(List<SOTeam> teams) {
        ClearTeamHolder();
        for (int i = 0; i < teams.Count; i++) {
            TeamSelectionButton button = Instantiate(_prefabTeamSelectionButton, _transformTeamHolder);
            button.Text.text = teams[i].TeamName;
            button.Index = i;
            button.Button.onClick.AddListener(()=>_libraryLogic.SelectTeamByIndex(button.Index));
            button.gameObject.SetActive(true);
        }
    }

    private void DisplayTeams(SOTeam team) {
        _bpCutom.interactable = team!=null;
        _bpDelete.interactable = team != null;
        if (team == null) {
            _txtTeamName.text = "";
            _txtDescription.text = "";
            return;
        }
        _txtTeamName.text = team.TeamName;
        _txtDescription.text = team.GetDescription();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
}