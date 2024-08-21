using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreQuest : MonoBehaviour
{
    [Header("Quest Parameters")]
    [SerializeField] private string title = "Empty Title";
    [SerializeField] private string description = "Empty Description";
    [SerializeField] private ScorableObjectTags scoreTag = ScorableObjectTags.Floor;
    [SerializeField] private int amountToFinish = 1;

    [Header("Quest References")]
    [SerializeField] private TextMeshProUGUI titleTMPText;
    [SerializeField] private TextMeshProUGUI descriptionTMPText;
    [SerializeField] private TextMeshProUGUI scoreTMPText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Slider progressSlider;

    private int currentAmount = 0;
    private Color originalColor = Color.white;
    private Color finishedColor = new Color(0.592f, 1.0f, 0.569f);

    private void Awake()
    {
        Setup();
        GameManager.Instance.OnLevelReset += OnQuestReset;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLevelReset -= OnQuestReset;
    }

    private void Setup()
    {
        titleTMPText.text = title;
        descriptionTMPText.text = description;
        scoreTMPText.text = $"0/{amountToFinish}";
        progressSlider.value = 0f;
    }

    public void TryAddScore(ScorableObjectTags givenTag)
    {
        if (givenTag != scoreTag || currentAmount >= amountToFinish)
            return;

        currentAmount++;
        UpdateSlider();
        UpdateToScoreAmount();

        if (currentAmount >= amountToFinish)
        {
            OnQuestFinish();
        }
    }

    private void UpdateSlider()
    {
        float targetValue = currentAmount == 0 ? 0 : currentAmount * 1f / amountToFinish * 1f;

        LeanTween.value(gameObject, UpdateSliderValue, progressSlider.value, targetValue, 0.5f)
            .setEase(LeanTweenType.easeInOutCubic)
            .setOnComplete(TryAnimatePopQuest);
    }
 
    private void UpdateSliderValue(float value)
    {
        progressSlider.value = value;
    }

    private void TryAnimatePopQuest()
    {
        if (currentAmount < amountToFinish)
            return;

        AnimateBackgroundColor(backgroundImage.color, finishedColor);
        AnimatePopQuest();
    }

    private void AnimatePopQuest()
    {
        Vector3 targetScale = Vector3.one;
        Vector3 startScale = new Vector3(0.8f, 0.8f, 0.8f);
        float duration = 0.5f; 

        transform.localScale = startScale;

        LeanTween.scale(gameObject, targetScale, duration)
            .setEase(LeanTweenType.easeInOutBack);
    }

    private void AnimateBackgroundColor(Color from, Color to)
    {
        float duration = 0.5f;

        LeanTween.value(gameObject,
            UpdateColor,
            from,
            to,
            duration 
        ).setEase(LeanTweenType.easeInOutQuad);
    }

    private void UpdateColor(Color color)
    {
        backgroundImage.color = color;
    }

    private void UpdateToScoreAmount()
    {
        scoreTMPText.text = $"{currentAmount}/{amountToFinish}";
    }

    private void OnQuestFinish()
    {
        ScoreManager.Instance.AddStar();
    }

    private void OnQuestReset()
    {
        currentAmount = 0;
        AnimateBackgroundColor(backgroundImage.color, originalColor);
        UpdateSlider();
        UpdateToScoreAmount();
    }
}
