using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorableObject : MonoBehaviour
{
    [SerializeField] private List<ScorableObjectTags> scorableTags = new List<ScorableObjectTags>();

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

    private void OnTriggerEnter(Collider other)
    {
        if (scored)
            return;

        foreach (ScorableObjectTags scorableTag in scorableTags)
        {
            if (other.gameObject.tag == scorableTag.ToString())
            {
                scored = true;
                ScoreManager.Instance.TryFindNeededQuest(scorableTag);
                break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (scored)
            return;

        foreach(ScorableObjectTags scorableTag in scorableTags)
        {
            if (collision.gameObject.tag == scorableTag.ToString())
            {
                scored = true;
                ScoreManager.Instance.TryFindNeededQuest(scorableTag);
                break;
            }
        }     
    }
}
