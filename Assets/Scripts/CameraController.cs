using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float rotationAngle = 90.0f; // Fixed rotation angle

    private void Update()
    {
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
        // Rotate the camera around the world center (Vector3.zero) on the Y-axis by the specified angle
        transform.RotateAround(Vector3.zero, Vector3.up, angle);
    }
}
