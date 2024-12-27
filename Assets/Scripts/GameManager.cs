using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private PlayerController player;
    [SerializeField] private bool isLastLevel = false;
    
    public event Action OnLevelReset;
    public event Action OnEscapePressed;

    private const string LEVEL_NAME = "Level_";

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetStage();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EscapePressed();
        }
    }

    private void EscapePressed()
    {
        OnEscapePressed?.Invoke();
    }

    public void ResetStage()
    {
        OnLevelReset?.Invoke();
    }

    public void WinStage()
    {
        Debug.Log("Stage Won!");
        PersistantData.Instance.SetLevelConpletedInfo(ScoreManager.Instance.GetCurrentStarAmount());
        PersistantData.Instance.SavePlayer();
        if(isLastLevel)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(LEVEL_NAME + (PersistantData.Instance.levelId + 1).ToString());
        }
    }
}
