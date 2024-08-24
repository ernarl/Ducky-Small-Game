using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPopScripts : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Vector3 pressedScale = Vector3.one * 0.9f;
    [SerializeField] private float animationTime = 0.2f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, pressedScale, animationTime).setEaseOutQuad();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale, animationTime).setEaseOutQuad();
    }
}
