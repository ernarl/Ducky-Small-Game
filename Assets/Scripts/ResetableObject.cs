using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetableObject : MonoBehaviour
{
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Rigidbody rb;

    private void Awake()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        GameManager.Instance.OnLevelReset += ResetGameObject;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLevelReset -= ResetGameObject;
    }

    private void ResetGameObject()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; 

        rb.interpolation = RigidbodyInterpolation.None; 

        transform.rotation = startingRotation;
        transform.position = startingPosition;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
