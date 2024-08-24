using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Image levelImage;
    [SerializeField] private Image[] starImages;

    private const string LEVEL_NAME = "Level_";
    private int index = -1;

    private void Awake()
    {
        button.onClick.AddListener(LoadLevelScene);
    }

    private void LoadLevelScene()
    {
        PersistantData.Instance.levelId = index;
        SceneManager.LoadScene(LEVEL_NAME + index.ToString());
    }

    public void SetLevelIndex(int _index)
    {
        index = _index;
    }

    public void SetLevelTitle(string _newTitle)
    {
        title.text = _newTitle;
    }

    public void SetLevelSprite(Sprite _newSprite)
    {
        levelImage.sprite = _newSprite;
    }

    public void SetStarSprites(int _amount, Sprite _newSprite)
    {
        for(int i = 0; i < _amount; i++)
        {
            starImages[i].sprite = _newSprite;
        }
    }

    public void SetUnlickable()
    {
        button.interactable = false;
    }
}
