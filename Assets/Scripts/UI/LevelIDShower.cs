using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelIDShower : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelIdText;

    private void Start()
    {
        if(PersistantData.Instance != null)
        {
            levelIdText.text = (PersistantData.Instance.levelId + 1).ToString();
        }
    }
}
