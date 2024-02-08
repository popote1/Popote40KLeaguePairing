using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchRecapUI : MonoBehaviour {
    [SerializeField] private List<MatchUI> _matchUIs;
    [SerializeField] private Button _bpClose;
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    
    private void Awake() {
        _bpClose.onClick.AddListener(Hide);
        Hide();
    }

    public void SetMatchCount(int count) {
        for (int i = 0; i < _matchUIs.Count; i++) {
            if (i < count) _matchUIs[i].Show();
            else _matchUIs[i].Hide();
        }        
    }

    public void SetCurrentMatch(List<Tuple<SOCarte, SOCarte>> matchs) {
        for (int i = 0; i < _matchUIs.Count; i++) {
            if (i >= matchs.Count) {
                _matchUIs[i].DisplayMatch(0, null, null);
            }
            else {
                _matchUIs[i].DisplayMatch(i, matchs[i].Item1, matchs[i].Item2);
            }
        }
        Show();
    }
}