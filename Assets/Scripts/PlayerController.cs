using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Moving")]
    [SerializeField] private float moveSpeed = 5f; // Speed of the player movement
    [SerializeField] private float maxSpeed = 5f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f; // Force applied to the jump
    [SerializeField] private LayerMask groundLayer; // Layer to identify what is considered ground
    [SerializeField] private Transform groundCheckTransform;

    private Rigidbody rb;
    private bool isGrounded;

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

        // Check if the player is on the ground
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, 0.23f, groundLayer);

        // Check for jump input
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // Apply upward force for jumping
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position to visualize the ground check sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheckTransform.position, 0.23f);
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }
}
