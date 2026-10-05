using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Restarts the level when the duck touches it. Works on solid objects (like the floor)
// and on triggers (like the plane under the map, so things that fall off the map fall through it instead of landing on it)
public class LosingColliderObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameManager.Instance.LoseStage();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameManager.Instance.LoseStage();
            return;
        }

        ScorableObject scorableObject = other.GetComponentInParent<ScorableObject>();
        if(scorableObject != null)
        {
            scorableObject.FellOutOfMap();
        }
    }
}
