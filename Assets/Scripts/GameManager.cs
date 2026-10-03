using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private PlayerController player;

    public event Action OnLevelReset;

    private const string LEVEL_NAME = "Level_";
    private const string LEVEL_SELECTION_SCENE = "LevelPicking";

    private void Awake()
    {
        Instance = this;
        SyncLevelIdWithScene();
    }

    // Take the level id from the scene name (Level_N) so it's always correct, even when a level is opened directly
    private void SyncLevelIdWithScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (PersistantData.Instance != null
            && sceneName.StartsWith(LEVEL_NAME)
            && int.TryParse(sceneName.Substring(LEVEL_NAME.Length), out int sceneLevelId))
        {
            PersistantData.Instance.levelId = sceneLevelId;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetStage();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToLevelSelection();
        }
    }

    private void ReturnToLevelSelection()
    {
        SceneTransition.LoadScene(LEVEL_SELECTION_SCENE);
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

        int nextLevelId = PersistantData.Instance.levelId + 1;
        string nextLevelScene = LEVEL_NAME + nextLevelId.ToString();
        if(Application.CanStreamedLevelBeLoaded(nextLevelScene))
        {
            PersistantData.Instance.levelId = nextLevelId;
            SceneTransition.LoadScene(nextLevelScene);
        }
        else
        {
            // No next level in the build, so this was the last one
            SceneTransition.LoadScene(1);
        }
    }
}
