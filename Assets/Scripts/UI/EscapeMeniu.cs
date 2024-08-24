using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            SceneManager.LoadScene(0);
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

        GameManager.Instance.OnEscapePressed += Instance_OnEscapePressed;
        Close();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnEscapePressed -= Instance_OnEscapePressed;
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

    private void Instance_OnEscapePressed()
    {
        if(gameObject.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
}
