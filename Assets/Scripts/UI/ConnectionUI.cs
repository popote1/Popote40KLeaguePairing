using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{

    [SerializeField] private Button _bpCreate;
    [SerializeField] private Button _bpJoin;
    [SerializeField] private Button _bpReturn;
    
    
    void Start() {
        _bpCreate.onClick.AddListener(PopoteNetPart.Instance.StartHost);
        _bpJoin.onClick.AddListener(PopoteNetPart.Instance.StartClient);
        _bpReturn.onClick.AddListener(Hide);
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }
    
    private void Hide() {
        gameObject.SetActive(false);
    }

    
}
