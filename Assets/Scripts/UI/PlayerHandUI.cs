using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerHandUI : MonoBehaviour
{

    public event EventHandler<int> OnCarteSelected;
    
    [SerializeField] private Transform _transformCarteHolder;
    [SerializeField] private CarteUI _prefabCarte;

    private List<SOCarte> _team;
    private List<CarteUI> _carts;
    
    public void ShowCartOfIndex(int id) =>_carts[id].gameObject.SetActive(true);
    public void HideCartOfIndex(int id) =>_carts[id].gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    
    private void Start() {
        Hide();
    }
    public void SetTeam(List<SOCarte> team) {
        ClearHead();
        _team = team;
        for (int i = 0; i < team.Count; i++) {
            CarteUI cart = Instantiate(_prefabCarte, _transformCarteHolder);
            cart.SetNewCarte(_team[i]);
            cart.BpCart.onClick.AddListener(()=>CarteClick(cart.GetSoCarte()));
            _carts.Add(cart);
        }
    }
    
    private void CarteClick(SOCarte carte) {
        if (!_team.Contains(carte)) return;
        int index = _team.IndexOf(carte);
        OnCarteSelected?.Invoke(this, index);
    }
    
    private void ClearHead() {
        for (int i = 0; i < _transformCarteHolder.childCount; i++) {
            if (_transformCarteHolder.GetChild(i) == _prefabCarte) continue;
            _transformCarteHolder.GetChild(i).gameObject.SetActive(false);
            Destroy(_transformCarteHolder.GetChild(i).gameObject, 0.1f);
        }

        _carts = new List<CarteUI>();
    }
    
    
}