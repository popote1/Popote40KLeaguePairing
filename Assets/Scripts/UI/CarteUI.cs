using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarteUI : MonoBehaviour
{
    public Button BpCart;
    [SerializeField] private Image _img;
    [SerializeField] private TMP_Text _txtName;
    [SerializeField] private SOCarte _soCarte;
    [SerializeField] private UIFeedBack _uiFeedback;

    private void Awake() {
        _uiFeedback = GetComponent<UIFeedBack>();
    }

    private void Start() {
        if (_uiFeedback!=null && _uiFeedback.enabled) _uiFeedback.OnPointerExit(null);
    }


    public SOCarte GetSoCarte() => _soCarte;
    [ContextMenu("Apply New SOCarte")]
    public void ApplyNewCarte() {
        _img.sprite = _soCarte.Sprite;
        _txtName.text = _soCarte.Name;
    }

    public void SetNewCarte(SOCarte soCarte) {
        if (soCarte == null) {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        _soCarte = soCarte;
        ApplyNewCarte();
    }
}