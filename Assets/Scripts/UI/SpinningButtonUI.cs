using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Shows a duck on each side of the hovered menu button. The ducks pop in with a twirl, glide over when another
// button is hovered, float like rubber ducks on water while shown, hop when the button is clicked and pop out when it's left
public class SpinningButtonUI : MonoBehaviour
{
    [SerializeField] private List<SpinTriggerButton> spinButtons = new List<SpinTriggerButton>();
    [SerializeField] private GameObject leftObject;
    [SerializeField] private GameObject rightObject;
    [SerializeField] private float offset = 80f;

    [Header("Showing")]
    [SerializeField] private float popInTime = 0.35f;
    [SerializeField] private float twirlTime = 0.6f;
    [SerializeField] private float popOutTime = 0.15f;
    [SerializeField] private float popOutDelay = 0.08f; // Moving the mouse between two buttons glides the ducks over instead of hiding them
    [SerializeField] private float moveTime = 0.2f;

    [Header("Floating")]
    [SerializeField] private float turnToButtonAngle = 55f; // 0 looks straight at the player, 90 is a side view looking at the button
    [SerializeField] private float bobHeight = 6f;
    [SerializeField] private float bobSpeed = 3f;
    [SerializeField] private float rockAngle = 8f;

    [Header("Hop On Click")]
    [SerializeField] private float hopHeight = 25f;
    [SerializeField] private float hopTime = 0.3f;

    private Transform[] ducks;
    private float[] duckSides; // 1 is right of the button, -1 is left
    private float[] duckBaseHeights;
    private Camera canvasCamera;

    private float halfButtonWidth;
    private float popScale;
    private float twirlAngle;
    private float hop;
    private int popTweenId = -1;
    private int twirlTweenId = -1;
    private int moveTweenId = -1;
    private int hopTweenId = -1;

    private void Awake()
    {
        ducks = new[] { leftObject.transform, rightObject.transform };
        duckSides = new[] { 1f, -1f };
        duckBaseHeights = new[] { ducks[0].localPosition.y, ducks[1].localPosition.y };
        canvasCamera = GetComponentInParent<Canvas>().rootCanvas.worldCamera;

        SetupButtons();
        TurnOff();
    }

    private void SetupButtons()
    {
        foreach(SpinTriggerButton spinButton in spinButtons)
        {
            EventTrigger trigger = spinButton.button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = spinButton.button.gameObject.AddComponent<EventTrigger>();
            }

            // Pointer Enter Entry
            EventTrigger.Entry enterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            enterEntry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data, spinButton.button); });

            // Pointer Exit Entry
            EventTrigger.Entry exitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            exitEntry.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });

            // Add entries to trigger
            trigger.triggers.Add(enterEntry);
            trigger.triggers.Add(exitEntry);

            spinButton.button.onClick.AddListener(Hop);
        }
    }

    private void Update()
    {
        for (int i = 0; i < ducks.Length; i++)
        {
            // The ducks float out of step with each other
            float time = Time.unscaledTime * bobSpeed + i * Mathf.PI;
            float height = duckBaseHeights[i] + Mathf.Sin(time) * bobHeight + Mathf.Sin(hop * Mathf.PI) * hopHeight;
            ducks[i].localPosition = new Vector3(duckSides[i] * (halfButtonWidth + offset), height, ducks[i].localPosition.z);

            ducks[i].rotation = GetLookRotation(ducks[i]) * Quaternion.Euler(0f, twirlAngle, Mathf.Sin(time * 0.5f) * rockAngle);
            ducks[i].localScale = Vector3.one * popScale;
        }
    }

    // Looks at the player, then turns towards the button. Worked out from where the camera really is, since the menu
    // camera has perspective: the same turn would show one duck from the front and the other from the side
    private Quaternion GetLookRotation(Transform duck)
    {
        Vector3 up = transform.up;
        Vector3 toCamera = Vector3.ProjectOnPlane(canvasCamera != null ? canvasCamera.transform.position - duck.position : -transform.forward, up);
        Vector3 toButton = transform.position - duck.position;

        // Which way around the up axis turns from the camera towards the button
        float turnSide = Mathf.Sign(Vector3.Dot(Vector3.Cross(toCamera, toButton), up));
        Vector3 facing = Quaternion.AngleAxis(turnSide * turnToButtonAngle, up) * toCamera;
        return Quaternion.LookRotation(facing, up);
    }

    private void OnPointerEnter(PointerEventData data, Button button)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (gameObject.activeSelf)
        {
            // Already showing (or just starting to hide after leaving another button)
            MoveTo(rectTransform);
            PopIn();
        }
        else
        {
            TurnOn(rectTransform);
        }
    }

    private void OnPointerExit(PointerEventData data)
    {
        LeanTween.cancel(popTweenId);
        popTweenId = LeanTween.value(gameObject, SetPopScale, popScale, 0f, popOutTime)
            .setEaseInBack()
            .setDelay(popOutDelay)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                popTweenId = -1;
                TurnOff();
            })
            .id;
    }

    public void TurnOff()
    {
        LeanTween.cancel(popTweenId);
        LeanTween.cancel(twirlTweenId);
        LeanTween.cancel(moveTweenId);
        LeanTween.cancel(hopTweenId);
        popScale = 0f;
        hop = 0f;
        gameObject.SetActive(false);
    }

    public void TurnOn(RectTransform rectTransform)
    {
        LeanTween.cancel(moveTweenId);
        gameObject.transform.position = rectTransform.position;
        halfButtonWidth = rectTransform.rect.width / 2f;
        popScale = 0f;
        gameObject.SetActive(true);

        PopIn();
        // A full turn while popping in, ending up looking at the button
        LeanTween.cancel(twirlTweenId);
        twirlTweenId = LeanTween.value(gameObject, SetTwirlAngle, 360f, 0f, twirlTime)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .id;
    }

    private void PopIn()
    {
        LeanTween.cancel(popTweenId);
        popTweenId = LeanTween.value(gameObject, SetPopScale, popScale, 1f, popInTime)
            .setEaseOutBack()
            .setIgnoreTimeScale(true)
            .id;
    }

    private void MoveTo(RectTransform rectTransform)
    {
        Vector3 fromPosition = gameObject.transform.position;
        float fromHalfWidth = halfButtonWidth;

        LeanTween.cancel(moveTweenId);
        moveTweenId = LeanTween.value(gameObject, t =>
            {
                gameObject.transform.position = Vector3.Lerp(fromPosition, rectTransform.position, t);
                halfButtonWidth = Mathf.Lerp(fromHalfWidth, rectTransform.rect.width / 2f, t);
            }, 0f, 1f, moveTime)
            .setEaseOutQuad()
            .setIgnoreTimeScale(true)
            .id;
    }

    private void Hop()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        LeanTween.cancel(hopTweenId);
        hopTweenId = LeanTween.value(gameObject, SetHop, 0f, 1f, hopTime)
            .setIgnoreTimeScale(true)
            .id;
    }

    private void SetPopScale(float value)
    {
        popScale = value;
    }

    private void SetTwirlAngle(float value)
    {
        twirlAngle = value;
    }

    private void SetHop(float value)
    {
        hop = value;
    }
}
