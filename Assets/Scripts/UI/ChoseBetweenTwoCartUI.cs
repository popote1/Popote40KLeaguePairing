using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoseBetweenTwoCartUI : MonoBehaviour {
    public event EventHandler<int> OnChosen;
    public event EventHandler OnSubmit;

    [SerializeField] private Button _bpSubmit;
    [Header("Carte1")] 
    [SerializeField] private TMP_Text _txtTexteader1;
    [SerializeField] private CarteUI _carteUI1;
    [SerializeField] private Button _bpChose1;
    [Header("Carte2")]
    [SerializeField] private TMP_Text _txtTexteader2;
    [SerializeField] private CarteUI _carteUI2;
    [SerializeField] private Button _bpChose2;
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    private void Awake() {
        _bpSubmit.onClick.AddListener(()=>OnSubmit?.Invoke(this, EventArgs.Empty));
        _bpChose1.onClick.AddListener(PickCarte1);
        _bpChose2.onClick.AddListener(PickCarte2);
    }
    private void Start() {
        Hide();
    }
    
    public void DisplayCarts(SOCarte carte1,SOCarte carte2) {
        _bpSubmit.interactable = false;
        _carteUI1.SetNewCarte(carte1);
        _carteUI2.SetNewCarte(carte2);
        _txtTexteader1.text = "";
        _txtTexteader2.text = "";
        Show();
    }

    public void PickCarte1() {
        OnChosen?.Invoke(this, 1);
        _bpSubmit.interactable = true;
        _txtTexteader1.text = "CHOSEN";
        _txtTexteader2.text = "RETURN";
    }
    public void PickCarte2() {
        OnChosen?.Invoke(this, 2);
        _bpSubmit.interactable = true;
        _txtTexteader1.text = "RETURN";
        _txtTexteader2.text = "CHOSEN";
    }
   
    
    
    
}