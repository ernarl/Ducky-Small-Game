using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            SceneManager.LoadScene("LevelPicking");
        });
    }
}
