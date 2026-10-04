using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LosingColliderObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            AudioManager.Play(SoundNames.LevelLose);
            GameManager.Instance.ResetStage();
        }
    }
}
