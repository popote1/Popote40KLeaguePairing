using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class LibraryLogic : MonoBehaviour
{

    public event EventHandler<List<SOTeam>> OnSavedTeamUpdate;
    public event EventHandler OnCloseTeamBuilding;
    public event EventHandler<SOTeam> OnOpenTeamBuilding;
    public event EventHandler<SOTeam> OnSelectTeam; 
    

    private SOTeam _currentTeam;

    private List<SOTeam> _SavedTeam;


    public void SelectCartFromLibrary(CarteUI carte) {
        _currentTeam.Factions.Add(carte.GetSoCarte().Name);
    }

    public void RemoveCarteFromTeam(CarteUI carte) {
        _currentTeam.Factions.Remove(carte.GetSoCarte().Name);
    }

    public void CreateNewTeam(string text) {
        if (String.IsNullOrEmpty(text)) text = "NewTeam";
        _currentTeam = ScriptableObject.CreateInstance<SOTeam>();
        _currentTeam.TeamName = text;
        _currentTeam.Factions = new List<string>();
        OnOpenTeamBuilding?.Invoke(this,_currentTeam);
    }

    public void CutomizTeam() {
        OnOpenTeamBuilding?.Invoke(this,_currentTeam);
    }

    public void SelectTeamByIndex(int id) {
        _currentTeam = _SavedTeam[id];
        OnSelectTeam.Invoke(this,_SavedTeam[id]);
    }

    public void CloseTeamBuilding() {
        OnCloseTeamBuilding?.Invoke(this , EventArgs.Empty);
    }

    public void SaveTeam() {
        SaveCurrentTeam();
        Debug.Log("Current Team Saved");
    }
    
    

    [ContextMenu("UpdateSavedTeams")]
    public void UpdateSavedTeams() {
        _SavedTeam =LoadSavedTeam();
        OnSavedTeamUpdate.Invoke(this , _SavedTeam);
        OnSelectTeam.Invoke(this  ,null);
    }
    
    private List<SOTeam> LoadSavedTeam() {
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

    private void SaveCurrentTeam() {
        string data =JsonUtility.ToJson(_currentTeam.CreateSerializationData());

        if (!Directory.Exists(Application.persistentDataPath)) {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
        File.WriteAllText(Application.persistentDataPath+"/"+_currentTeam.TeamName+".txt", data);
    }

    public void DeleteSelectedTeam() {
        string pathToDesctroy = null;
        foreach (string path in Directory.GetFiles(Application.persistentDataPath)) {
            try {
                SerializeSOTeam data = JsonUtility.FromJson<SerializeSOTeam>(File.ReadAllText(path));
                if (data.TeamName == _currentTeam.TeamName) {
                    pathToDesctroy = path;
                    break;
                }
            }
            catch (Exception e) {
            }
        }
        if( pathToDesctroy!=null) File.Delete(pathToDesctroy);
        UpdateSavedTeams();
    }
}
