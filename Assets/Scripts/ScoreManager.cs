using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    [SerializeField] private List<ScoreQuest> scoreQuests = new List<ScoreQuest>();

    public const int MAX_STARS = 3;

    private void Awake()
    {
        Instance = this;

        if(scoreQuests.Count > MAX_STARS)
        {
            Debug.LogError("Wrong scoreQuest amount!");
        }
    }

    public void TryFindNeededQuest(ScorableObjectTags givenTag)
    {
        foreach(ScoreQuest quest in scoreQuests)
        {
            quest.TryAddScore(givenTag);
        }
    }

    // All quests finished gives 3 stars, every unfinished quest takes one away
    public int GetCurrentStarAmount()
    {
        int unfinishedQuests = 0;
        foreach(ScoreQuest quest in scoreQuests)
        {
            if(!quest.IsFinished())
            {
                unfinishedQuests++;
            }
        }

        return Mathf.Max(0, MAX_STARS - unfinishedQuests);
    }
}
