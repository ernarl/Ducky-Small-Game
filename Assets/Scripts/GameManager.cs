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
    private const string MAIN_SCENE = "MainScene";
    private const string ALL_STARS_MESSAGE = "Thanks for playing!\n<size=50%>You got every star</size>";
    private const string MISSING_STARS_MESSAGE = "You finished the last level!\n<size=50%>But some stars are still missing.\nCollect them all to find a hidden message</size>";

    private int lastLoseFrame = -1;

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
            AudioManager.Play(SoundNames.LevelRestart);
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

    public void LoseStage()
    {
        // Touching two losing objects at once (like landing between two floor tiles) should only restart once
        if (lastLoseFrame == Time.frameCount)
        {
            return;
        }
        lastLoseFrame = Time.frameCount;

        AudioManager.Play(SoundNames.LevelLose);
        ResetStage();
    }

    public void WinStage()
    {
        Debug.Log("Stage Won!");
        AudioManager.Play(SoundNames.LevelWin);

        int levelCount = GetLevelCount();
        bool hadAllStars = PersistantData.Instance.HasMaxStarsOnLevels(levelCount);

        PersistantData.Instance.SetLevelConpletedInfo(ScoreManager.Instance.GetCurrentStarAmount());
        PersistantData.Instance.SavePlayer();

        // Getting the last missing star thanks the player (only once, since stars are never lost),
        // then the duck flies into the camera and the game goes back to the main menu
        if (!hadAllStars && PersistantData.Instance.HasMaxStarsOnLevels(levelCount))
        {
            SceneTransition.LoadScene(MAIN_SCENE, ALL_STARS_MESSAGE, DuckFinale.Play);
            return;
        }

        int nextLevelId = PersistantData.Instance.levelId + 1;
        string nextLevelScene = LEVEL_NAME + nextLevelId.ToString();
        if(Application.CanStreamedLevelBeLoaded(nextLevelScene))
        {
            PersistantData.Instance.levelId = nextLevelId;
            SceneTransition.LoadScene(nextLevelScene);
        }
        else if (PersistantData.Instance.HasMaxStarsOnLevels(levelCount))
        {
            // No next level in the build, so this was the last one (replayed after getting every star)
            SceneTransition.LoadScene(LEVEL_SELECTION_SCENE);
        }
        else
        {
            // The last level, but not everything is done yet: hint that getting every star unlocks the thank-you message
            SceneTransition.LoadScene(LEVEL_SELECTION_SCENE, MISSING_STARS_MESSAGE);
        }
    }

    // The levels are the Level_N scenes in the build, numbered from 0 without gaps
    private int GetLevelCount()
    {
        int count = 0;
        while (Application.CanStreamedLevelBeLoaded(LEVEL_NAME + count))
        {
            count++;
        }
        return count;
    }
}
