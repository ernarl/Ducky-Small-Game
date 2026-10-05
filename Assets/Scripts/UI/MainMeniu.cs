using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMeniu : BasicMeniu
{
    [Header("Menius")]
    [SerializeField] private OptionsMeniu optionsMeniu;
    [SerializeField] private QuitConfimationPopup quitConfirmationPopup;

    [Header("Buttons")]
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button playbutton;

    private void Awake()
    {
        // A browser game can't close itself, so there's nothing for the quit button to do
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitButton.gameObject.SetActive(false);
        }

        quitButton.onClick.AddListener(() =>
        {
            quitConfirmationPopup.Open();
           /* quitConfirmationPopup.onClosed += () =>
            {
                Open();
            };*/
        });

        optionsButton.onClick.AddListener(() =>
        {
            optionsMeniu.Open();
        });

        playbutton.onClick.AddListener(() =>
        {
            SceneTransition.LoadScene("LevelPicking");
        });
    }
}
