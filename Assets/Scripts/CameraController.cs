using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationAngle = 90.0f; // Fixed rotation angle
    [SerializeField] private float rotationDuration = 0.4f;
    [SerializeField] private LeanTweenType rotationEase = LeanTweenType.easeOutCubic;

    private Vector3 mapCenter;
    private Vector3 startOffset; // Camera position relative to the map center, before any rotation
    private Quaternion startRotation;
    private float currentAngle;
    private float targetAngle;
    private int rotationTweenId = -1;

    private void Awake()
    {
        startRotation = transform.rotation;
        mapCenter = FindMapCenter();

        // Slide the camera (keeping its angle and distance) so it looks straight at the map center.
        // Turning around that point then keeps the map in the middle of the screen from every side
        float distance = Vector3.Dot(mapCenter - transform.position, transform.forward);
        startOffset = -transform.forward * distance;
        transform.position = mapCenter + startOffset;
    }

    // The middle of everything visible in the level. The duck is left out, since it moves around
    private Vector3 FindMapCenter()
    {
        Bounds mapBounds = new Bounds();
        bool foundAny = false;
        foreach (MeshRenderer meshRenderer in FindObjectsOfType<MeshRenderer>())
        {
            if (!meshRenderer.enabled || meshRenderer.GetComponentInParent<PlayerController>() != null)
            {
                continue;
            }

            if (foundAny)
            {
                mapBounds.Encapsulate(meshRenderer.bounds);
            }
            else
            {
                mapBounds = meshRenderer.bounds;
                foundAny = true;
            }
        }

        return mapBounds.center;
    }

    private void Update()
    {
        // Tweens don't advance while paused, so don't queue up rotations
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateLeft();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateRight();
        }
    }

    private void RotateLeft()
    {
        RotateAroundMapCenter(-rotationAngle);
    }

    private void RotateRight()
    {
        RotateAroundMapCenter(rotationAngle);
    }

    private void RotateAroundMapCenter(float angle)
    {
        // Stack onto the target so quick repeated presses keep turning from where the camera currently is
        targetAngle += angle;
        AudioManager.Play(SoundNames.CameraRotate);

        LeanTween.cancel(rotationTweenId);
        rotationTweenId = LeanTween.value(gameObject, SetAngle, currentAngle, targetAngle, rotationDuration)
            .setEase(rotationEase)
            .id;
    }

    private void SetAngle(float angle)
    {
        // Rotate the camera around the map center on the Y-axis by the given angle
        currentAngle = angle;
        Quaternion orbit = Quaternion.Euler(0f, angle, 0f);
        transform.SetPositionAndRotation(mapCenter + orbit * startOffset, orbit * startRotation);
    }
}
