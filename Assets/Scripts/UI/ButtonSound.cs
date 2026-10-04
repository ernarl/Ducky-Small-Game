using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Plays the click / hover sounds of the button it's on.
// The AudioManager adds it to every button when a scene loads, so it only has to be added by hand
// to give a specific button different sounds.
[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private string clickSound = SoundNames.ButtonClick;
    [SerializeField] private string hoverSound = SoundNames.ButtonHover;
    [SerializeField] private string lockedSound = SoundNames.ButtonLocked;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        // onClick instead of OnPointerClick, so it also plays when the button is pressed with the keyboard
        button.onClick.AddListener(() => AudioManager.Play(clickSound));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.IsInteractable())
        {
            AudioManager.Play(hoverSound);
        }
    }

    // A button that isn't interactable (like a locked level) doesn't call onClick, so let the player hear it's locked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!button.IsInteractable())
        {
            AudioManager.Play(lockedSound);
        }
    }
}
