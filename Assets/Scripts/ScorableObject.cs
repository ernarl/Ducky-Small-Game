using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorableObject : MonoBehaviour
{
    [SerializeField] private List<ScorableObjectTags> scorableTags = new List<ScorableObjectTags>();
    [SerializeField] private float fallDistanceToScore = 0f; // Dropping this far below where it started counts as landing on the floor (0 turns it off)

    private bool scored = false;
    private float startHeight;

    private void Start()
    {
        startHeight = transform.position.y;
        GameManager.Instance.OnLevelReset += ResetScoring;
    }

    // Knocked down from its place counts even when it lands on something that isn't the floor (a mat, the trash can, slippers...)
    private void FixedUpdate()
    {
        if (fallDistanceToScore > 0f && startHeight - transform.position.y >= fallDistanceToScore)
        {
            TryScore(ScorableObjectTags.Floor.ToString());
        }
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
        TryScore(other.gameObject.tag);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryScore(collision.gameObject.tag);
    }

    // Falling out of the map counts the same as landing on the floor, so an object knocked off the edge still counts
    public void FellOutOfMap()
    {
        TryScore(ScorableObjectTags.Floor.ToString());
    }

    // The duck's collider is on a child object, and Unity sends the trigger message to the object with the Rigidbody
    // instead, so this never gets OnTriggerEnter from the water by itself. The water calls this instead
    public void ReachedWater()
    {
        TryScore(ScorableObjectTags.Water.ToString());
    }

    private void TryScore(string touchedTag)
    {
        if (scored)
            return;

        foreach (ScorableObjectTags scorableTag in scorableTags)
        {
            if (touchedTag == scorableTag.ToString())
            {
                scored = true;
                ScoreManager.Instance.TryFindNeededQuest(scorableTag);
                break;
            }
        }
    }
}
