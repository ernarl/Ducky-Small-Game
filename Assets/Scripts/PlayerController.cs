using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Speed of the player movement
    [SerializeField] private float maxSpeed = 5f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Get input from WASD or arrow keys
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Create a movement vector based on the input
        Vector3 moveDirection = new Vector3(moveX, 0, moveY);

        // Normalize the direction vector to ensure consistent movement speed
        moveDirection.Normalize();

        // Apply force to the Rigidbody in the direction of the input
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);

        // Check if the player's speed exceeds the maximum speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            // Clamp the velocity to the maxSpeed
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }
}
