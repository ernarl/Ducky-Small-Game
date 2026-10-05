using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPopScripts : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector3 pressedScale = Vector3.one * 0.9f;
    [SerializeField] private float animationTime = 0.2f;

    [Header("Hover")]
    [SerializeField] private bool scaleOnHover = false;
    [SerializeField] private Vector3 hoverScale = Vector3.one * 1.1f;

    private Vector3 originalScale;
    private Selectable selectable;
    private bool hovered = false;
    private bool pressed = false;
    private int scaleTweenId = -1;

    private void Awake()
    {
        originalScale = transform.localScale;
        selectable = GetComponent<Selectable>();
    }

    // A menu can close while its button is still pressed or hovered, so it shouldn't stay scaled the next time it opens
    private void OnDisable()
    {
        hovered = false;
        pressed = false;
        LeanTween.cancel(scaleTweenId);
        scaleTweenId = -1;
        transform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        AnimateScale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        AnimateScale();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        AnimateScale();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        AnimateScale();
    }

    private void AnimateScale()
    {
        // Ignores time scale so it still animates in the pause menu
        LeanTween.cancel(scaleTweenId);
        scaleTweenId = LeanTween.scale(gameObject, GetTargetScale(), animationTime)
            .setEaseOutQuad()
            .setIgnoreTimeScale(true)
            .id;
    }

    private Vector3 GetTargetScale()
    {
        if (pressed)
        {
            return pressedScale;
        }

        // Locked buttons don't grow on hover, so they don't look clickable
        bool interactable = selectable == null || selectable.IsInteractable();
        if (hovered && scaleOnHover && interactable)
        {
            return hoverScale;
        }

        return originalScale;
    }
}
