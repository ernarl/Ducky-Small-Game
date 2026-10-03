using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMeniu : BasicMeniu
{
    [SerializeField] private Button goBackButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button playButton;

    private void Awake()
    {
        goBackButton.onClick.AddListener(() =>
        {
            SceneTransition.LoadScene(0);
        });
        restartButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ResetStage();
            Close();
        });
        playButton.onClick.AddListener(() =>
        {
            Close();
        });

        Close();
    }

    public override void Open()
    {
        Time.timeScale = 0;
        base.Open();
    }

    public override void Close()
    {
        Time.timeScale = 1;
        base.Close();
    }
}
