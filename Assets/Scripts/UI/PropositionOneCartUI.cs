using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PropositionOneCartUI : MonoBehaviour
{
    public event EventHandler OnReturn;
    public event EventHandler OnSubmit;

    [SerializeField] private CarteUI _carteUI;
    [SerializeField] private Button _bpReturn;
    [SerializeField] private Button _bpSubmit;

    public void Show() {
        gameObject.SetActive(true);
    } 
    public void Hide() => gameObject.SetActive(false);

    public void DisplayCart(SOCarte carte) {
        _bpSubmit.interactable = !(carte == null);
        _carteUI.SetNewCarte(carte);
    }

    private void Awake() {
        _bpReturn.onClick.AddListener(()=>OnReturn?.Invoke(this, EventArgs.Empty));
        _bpSubmit.onClick.AddListener(()=>OnSubmit?.Invoke(this, EventArgs.Empty));
    }

    public void ClearShow()
    {
        DisplayCart(null);
        Show();
    }

    private void Start() {
        Hide();
    }
}