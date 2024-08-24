using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpinningButtonUI : MonoBehaviour
{
    [SerializeField] private List<SpinTriggerButton> spinButtons = new List<SpinTriggerButton>();
    [SerializeField] private GameObject leftObject;
    [SerializeField] private GameObject rightObject;
    [SerializeField] private float offset = 80f;

    private void Awake()
    {
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
        }
    }

    private void OnPointerEnter(PointerEventData data, Button button)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        TurnOn(rectTransform);
    }

    private void OnPointerExit(PointerEventData data)
    {
        TurnOff();
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void TurnOn(RectTransform rectTransform)
    {
        gameObject.transform.position = rectTransform.position;
        leftObject.transform.localPosition = new Vector3(rectTransform.rect.width / 2 + offset, leftObject.transform.localPosition.y, leftObject.transform.localPosition.z);
        rightObject.transform.localPosition = new Vector3(-rectTransform.rect.width / 2 - offset, rightObject.transform.localPosition.y, rightObject.transform.localPosition.z);
        gameObject.SetActive(true);
    }
}
