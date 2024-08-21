using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    [SerializeField] private List<ScoreQuest> scoreQuests = new List<ScoreQuest>();

    private int starAmount = 0;

    private void Awake()
    {
        Instance = this;
        GameManager.Instance.OnLevelReset += ResetStars;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLevelReset -= ResetStars;
    }

    public void TryFindNeededQuest(ScorableObjectTags givenTag)
    {
        foreach(ScoreQuest quest in scoreQuests)
        {
            quest.TryAddScore(givenTag);
        }
    }

    public void AddStar()
    {
        starAmount++;
        Debug.Log($"Current star amount: {starAmount}");
    }

    private void ResetStars()
    {
        starAmount = 0;
    }
}
