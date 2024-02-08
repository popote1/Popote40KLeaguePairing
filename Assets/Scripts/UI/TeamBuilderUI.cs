using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TeamBuilderUI : MonoBehaviour
{
    [SerializeField] private LibraryLogic _libraryLogic;
    [SerializeField] private TeamSelectorLibrary _teamSelectorLibrary;
    [SerializeField] private PanelBibliotequeUI _library;
    [SerializeField] private CarteUI _selectedUICarte;
    [SerializeField] private PanelBibliotequeUI _teamLibrary;
    [SerializeField] private Button _bpSave;
    [SerializeField] private Button _bpReturn;
    [SerializeField] private Button _bpRemoveCarte;
    [SerializeField] private List<SOCarte> _cartes;

    private CarteUI _selectedCart;

    private event EventHandler<CarteUI> OnCartSelectedFromLibrary;
    private event EventHandler<CarteUI> OnCartSelectedFromTeam; 

    private void Awake() {
        _bpReturn.onClick.AddListener(()=>_libraryLogic.CloseTeamBuilding());
        _bpSave.onClick.AddListener(()=>_libraryLogic.SaveTeam());
        _bpRemoveCarte.onClick.AddListener(() => RemoveCarteFromTeam());
        _libraryLogic.OnOpenTeamBuilding += LibraryLogic_OnOpenTeamBuilding;
        _library.OnCartSelected += Library_CarteSelected;
        _teamLibrary.OnCartSelected += TeamLibrary_CarteSelected;
        Hide();
        
    }

    private void RemoveCarteFromTeam() {
        _library.SwitchCarteToLibrary(_selectedCart);
        SetSelectedCarte(_selectedCart, false);
        _libraryLogic.RemoveCarteFromTeam(_selectedCart);
    }

    private void TeamLibrary_CarteSelected(object sender, CarteUI carte) {
        OnCartSelectedFromTeam?.Invoke(this, carte);
        SetSelectedCarte(carte, true);
    }

    private void Library_CarteSelected(object sender, CarteUI carte) {
        OnCartSelectedFromLibrary?.Invoke(this, carte);
        SetSelectedCarte(carte, true);
        _teamLibrary.SwitchCarteToLibrary(carte);
        _libraryLogic.SelectCartFromLibrary(carte);
    }

    private void SetSelectedCarte(CarteUI carte , bool isInTeam = false) {
        _selectedCart = carte;
        if (carte == null) {
            _selectedCart = null;
            _selectedUICarte.SetNewCarte(null);
        }
        else {
            _selectedCart = carte;
            _selectedUICarte.SetNewCarte(_selectedCart.GetSoCarte());
        };

        _bpRemoveCarte.interactable = isInTeam;

    }

    private void LibraryLogic_OnOpenTeamBuilding(object sender, SOTeam team) {
        Show();
        SetSelectedCarte(null);
        
        List<SOCarte> libraryCarts = new List<SOCarte>();
        List<SOCarte> teamCarts = new List<SOCarte>();
        for (int i = 0; i < _cartes.Count ;i++) {
            if (team.Factions.Contains(_cartes[i].Name)) teamCarts.Add(_cartes[i]);
            else libraryCarts.Add(_cartes[i]);
            //if( i<10)teamCarts.Add(_cartes[i]);
            //else libraryCarts.Add(_cartes[i]);
        }
        _teamLibrary.PopulateLibrary(teamCarts);
        _library.PopulateLibrary(libraryCarts);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private bool ContainThisCarte(List<SOCarte> carts, string name) {
        foreach (var cart in carts) if (cart.Name == name) return true;
        return false;

    }
}
