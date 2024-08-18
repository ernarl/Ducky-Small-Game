using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningColliderObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameManager.Instance.WinStage();
        }
    }
}
