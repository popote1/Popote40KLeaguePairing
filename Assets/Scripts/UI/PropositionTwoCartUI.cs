using System;
using UnityEngine;
using UnityEngine.UI;

public class PropositionTwoCartUI : MonoBehaviour
{
    public event EventHandler OnReturn1;
    public event EventHandler OnReturn2;
    public event EventHandler OnSubmit;

    [SerializeField] private Button _bpSubmit;
    [Header("Carte1")]
    [SerializeField] private CarteUI _carteUI1;
    [SerializeField] private Button _bpReturn1;
    [Header("Carte2")]
    [SerializeField] private CarteUI _carteUI2;
    [SerializeField] private Button _bpReturn2;
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    private void Awake() {
        _bpReturn1.onClick.AddListener(()=>OnReturn1?.Invoke(this, EventArgs.Empty));
        _bpReturn2.onClick.AddListener(()=>OnReturn2?.Invoke(this, EventArgs.Empty));
        _bpSubmit.onClick.AddListener(()=>OnSubmit?.Invoke(this, EventArgs.Empty));
        DisplayCart1(null);
        DisplayCart2(null);
        
    }
    private void Start() {
        Hide();
        CheckIfCanSubmite();
    }
    public void DisplayCart1(SOCarte carte) {
        _carteUI1.SetNewCarte(carte);
        CheckIfCanSubmite();
    }
    public void DisplayCart2(SOCarte carte) {
        _carteUI2.SetNewCarte(carte);
        CheckIfCanSubmite();
    }

    public void ClearShow() {
        DisplayCart1(null);
        DisplayCart2(null);
        Show();
    }
    private void CheckIfCanSubmite() {
        _bpSubmit.interactable = (_carteUI1.gameObject.activeSelf && _carteUI2.gameObject.activeSelf);
    }
}