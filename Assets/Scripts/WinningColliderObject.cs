using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningColliderObject : MonoBehaviour
{
    [SerializeField] private ParticleSystem waterSplashParticles;

    private bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !triggered)
        {
            triggered = true;
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            Instantiate(waterSplashParticles, collisionPoint, Quaternion.identity);
            GameManager.Instance.WinStage();
        }
    }
}
