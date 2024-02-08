using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIFeedBack : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public bool DoFeedBack = true;
    [SerializeField] private float _popupTime =0.3f;
    [SerializeField] private float _popupScale =1.3f;
    //[SerializeField] private Vector3 _popupRotation =new Vector3(5,0,0);
    [SerializeField] private AnimationCurve _popupAnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!DoFeedBack) return;
        transform.DOPause();
        transform.DOScale(Vector3.one * _popupScale, _popupTime).SetEase(_popupAnimationCurve);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!DoFeedBack) return;
        transform.DOPause();
        transform.DOScale(Vector3.one, _popupTime).SetEase(_popupAnimationCurve);
    }
}
