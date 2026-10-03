using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationAngle = 90.0f; // Fixed rotation angle
    [SerializeField] private float rotationDuration = 0.4f;
    [SerializeField] private LeanTweenType rotationEase = LeanTweenType.easeOutCubic;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float currentAngle;
    private float targetAngle;
    private int rotationTweenId = -1;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
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
        RotateAroundWorldCenter(-rotationAngle);
    }

    private void RotateRight()
    {
        RotateAroundWorldCenter(rotationAngle);
    }

    private void RotateAroundWorldCenter(float angle)
    {
        // Stack onto the target so quick repeated presses keep turning from where the camera currently is
        targetAngle += angle;

        LeanTween.cancel(rotationTweenId);
        rotationTweenId = LeanTween.value(gameObject, SetAngle, currentAngle, targetAngle, rotationDuration)
            .setEase(rotationEase)
            .id;
    }

    private void SetAngle(float angle)
    {
        // Rotate the camera around the world center (Vector3.zero) on the Y-axis by the given angle
        currentAngle = angle;
        Quaternion orbit = Quaternion.Euler(0f, angle, 0f);
        transform.SetPositionAndRotation(orbit * startPosition, orbit * startRotation);
    }
}
