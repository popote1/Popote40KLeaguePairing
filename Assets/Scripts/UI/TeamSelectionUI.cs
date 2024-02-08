using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionUI : MonoBehaviour
{

   public event EventHandler<SOTeam> OnConfirmSelectTeam;
   
   [SerializeField] private Transform _transformTeamHolder;
   [SerializeField] private Button _bpReturn;
   [SerializeField] private Button _bpSelect;
   [SerializeField] private TMP_Text _txtTeamName;
   [SerializeField] private TMP_Text _txtTeamComposition;
   [SerializeField] private TeamSelectionButton _prefabTeamSelectionButton;
   
   private List<SOTeam> _teams;
   private SOTeam _selectedTeam;
   private void Awake() {
      _bpReturn.onClick.AddListener(Hide);
      _bpSelect.onClick.AddListener(SelectTeam);
      Hide();
   }

   public void Hide() =>gameObject.SetActive(false);

   public void Show()
   {
      gameObject.SetActive(true);
   }

   private void SelectTeam()=> OnConfirmSelectTeam.Invoke(this , _selectedTeam);

   private void ClearTeamHolder() { 
      for (int i = 0; i < _transformTeamHolder.childCount; i++) {
         _transformTeamHolder.GetChild(i).gameObject.SetActive(false);
         if (_transformTeamHolder.GetChild(i) == _prefabTeamSelectionButton.transform) continue;
         Destroy(_transformTeamHolder.GetChild(i).gameObject, 0.1f);
            
      }
   }

   public void SetTeams(List<SOTeam> teams) {
      _teams = teams;
      ClearTeamHolder();
      for (int i = 0; i < teams.Count; i++) {
         TeamSelectionButton button = Instantiate(_prefabTeamSelectionButton, _transformTeamHolder);
         button.Text.text = teams[i].TeamName;
         button.Index = i;
         button.Button.interactable = teams[i].Factions.Count == 6;
         button.Button.onClick.AddListener(()=>SelectTeam(button.Index));
         button.gameObject.SetActive(true);
      }
      DisplayTeams(null);
   }

   private void DisplayTeams(SOTeam team) {
      _bpSelect.interactable = team!=null;
      if (team == null) {
         _txtTeamName.text = "";
         _txtTeamComposition.text = "";
         return;
      }
      _txtTeamName.text = team.TeamName;
      _txtTeamComposition.text = team.GetDescription();
   }

   private void SelectTeam(int id) {
      _selectedTeam = _teams[id];
      DisplayTeams(_selectedTeam);
   }
}
