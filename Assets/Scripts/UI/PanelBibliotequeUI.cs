using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBibliotequeUI : MonoBehaviour
{
    [SerializeField] private Transform _holder;
    [SerializeField] private CarteUI _prefabsCarteUI;
    [SerializeField] private SOCarte[] _soCartes;

    public event EventHandler<CarteUI> OnCartSelected;

    private void Start() {
        //ClearLibrary();
        //PopulateLibrary();
    }

    public void CarteSelected(CarteUI carteUI) {
        OnCartSelected?.Invoke(this, carteUI);
    }
    
   [ContextMenu(" PopulateLibrary")]
    private void PopulateLibrary(){
        Debug.Log("Do that");
        foreach (SOCarte soCarte in _soCartes) {

            if (soCarte == null)
            {
                Debug.Log("so Carte == null");
                continue;
            }
            CarteUI carte = Instantiate(_prefabsCarteUI, _holder);
            carte.SetNewCarte(soCarte);
            carte.BpCart.onClick.AddListener(()=>CarteSelected(carte));
        }
    }

    public void PopulateLibrary(List<SOCarte> carts) {
        _soCartes = carts.ToArray();
        Debug.Log(carts.Count);
        ClearLibrary();

        PopulateLibrary();
        
    }

    public void SwitchCarteToLibrary(CarteUI cart) {
        cart.transform.SetParent(_holder);
        cart.BpCart.onClick.RemoveAllListeners();
        cart.BpCart.onClick.AddListener(()=>CarteSelected(cart));
    }
    

    [ContextMenu(" ClearLibrary")]
    private void ClearLibrary() {
        foreach (Transform carte in _holder) {
            carte.gameObject.SetActive(false);
            Destroy(carte.gameObject, 0.01f);
        }
    }
}
