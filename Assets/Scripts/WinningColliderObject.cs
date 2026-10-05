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
            AudioManager.Play(SoundNames.Splash);

            // Finish the "Reach water" quest before winning, so it's counted in the stars
            ScorableObject scorableObject = other.GetComponentInParent<ScorableObject>();
            if (scorableObject != null)
            {
                scorableObject.ReachedWater();
            }
            GameManager.Instance.WinStage();
        }
    }
}
