using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewTeamUI : MonoBehaviour
{

    [SerializeField] private TeamSelectorLibrary _teamSelectorLibrary;
    [SerializeField] private LibraryLogic _libraryLogic;
    [SerializeField]private Button _bpReturn;
    [SerializeField]private Button _bpCreate;
    [SerializeField]private TMP_InputField _inputFieldName;

    private void Awake() {
        _bpReturn.onClick.AddListener(()=> Hide());
        _bpCreate.onClick.AddListener(()=> {
            CreateNewTeam();
            Hide();
        });
        Hide();
        _teamSelectorLibrary.OnButtonCreateNewTeamClicked += TeamSelectionLibrary_OnCreateNewTeam;
    }

    private void TeamSelectionLibrary_OnCreateNewTeam(object sender, EventArgs e) {
        _inputFieldName.text = "NewTeam";
        Show();
    }

    private void CreateNewTeam() {
        _libraryLogic.CreateNewTeam(_inputFieldName.text);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
