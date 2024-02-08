using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SubmitedPanelUI : MonoBehaviour {
    [SerializeField] private CarteUI _carteUI;
    
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    public void DisplayCart1(SOCarte carte) => _carteUI.SetNewCarte(carte);

    public void ClearShow() {
        _carteUI.SetNewCarte(null);
        Show();
    } 

    private void Start() {
        Hide();
    }
}
