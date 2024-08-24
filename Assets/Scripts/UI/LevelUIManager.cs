using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] private List<LevelButton> levelButtons = new List<LevelButton>();
    [SerializeField] private Sprite levelLockedSprite;
    [SerializeField] private Sprite levelCompletedSprite;
    [SerializeField] private Sprite starClaimedSprite;

    private void Awake()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        int i = 0;
        bool lastWasFinished = true;
        foreach(LevelButton levelButton in levelButtons)
        {
            levelButton.SetLevelIndex(i);
            levelButton.SetLevelTitle((i+1).ToString());

            if(PersistantData.Instance.playerData.LevelFinished[i])
            {
                lastWasFinished = true;
                levelButton.SetLevelSprite(levelCompletedSprite);
                levelButton.SetStarSprites(PersistantData.Instance.playerData.LevelStars[i], starClaimedSprite);
            }        
            else
            {
                if(lastWasFinished)
                {
                    lastWasFinished = false;                  
                }
                else
                {
                    levelButton.SetUnlickable();
                    levelButton.SetLevelSprite(levelLockedSprite);
                }
            }
            i++;
        }
    }
}
