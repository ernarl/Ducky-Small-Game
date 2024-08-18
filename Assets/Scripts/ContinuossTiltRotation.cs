using UnityEngine;

public class ContinuossTiltRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 20f;

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}

