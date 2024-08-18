using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorableObject : MonoBehaviour
{
    [SerializeField] private string interactToScoreTag = "empty";

    private bool scored = false;

    private void Start()
    {
        GameManager.Instance.OnLevelReset += ResetScoring;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLevelReset -= ResetScoring;
    }

    private void ResetScoring()
    {
        scored = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (scored)
            return;

        if(collision.gameObject.tag == interactToScoreTag)
        {
            scored = true;
            ScoreManager.Instance.TryFindNeededQuest(gameObject.tag);
        }
    }
}
