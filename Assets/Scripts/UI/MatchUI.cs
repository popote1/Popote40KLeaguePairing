using TMPro;
using UnityEngine;

public class MatchUI : MonoBehaviour {
    [SerializeField] private TMP_Text _txtMatchLabbel;
    [SerializeField] private CarteUI _carteUI1;
    [SerializeField] private CarteUI _carteUI2;
   

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    

    public void DisplayMatch(int count, SOCarte carte1, SOCarte carte2) {

        if (carte1 == null || carte2 == null) {
            Hide();
            return;
        }
        _txtMatchLabbel.text = "MATCH" + count;
        _carteUI1.SetNewCarte(carte1);
        _carteUI2.SetNewCarte(carte2);
        Show();
    }
}