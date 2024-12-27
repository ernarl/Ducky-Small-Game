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

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem jumpParticlesPrefab;

    private bool isGrounded;
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraForward.Normalize();

        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * moveY) + (cameraRight * moveX);
        moveDirection.Normalize();
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }


        // Sometiems doesnt jump when expected to

        // Define the ground check sphere's radius
        float sphereRadius = 0.23f;
        Vector3 groundCheckPos = groundCheckTransform.position;

        // Get all colliders in the ground check area
        Collider[] colliders = Physics.OverlapSphere(groundCheckPos, sphereRadius, groundLayer);

        isGrounded = false; // Default to not grounded

        // Loop through all colliders to check for valid ground
        foreach (Collider collider in colliders)
        {
            // Get the bounds of the collider
            Bounds colliderBounds = collider.bounds;

            // Check if the player's feet (groundCheckPos) are above the top surface of the collider
            if (groundCheckPos.y > colliderBounds.max.y)
            {
                // Try a raycast first
                RaycastHit hit;
                Vector3 rayOrigin = groundCheckPos + Vector3.up * 0.1f;
                if (Physics.Raycast(rayOrigin, Vector3.down, out hit, sphereRadius + 0.2f, groundLayer))
                {
                    if (Vector3.Angle(hit.normal, Vector3.up) < 45f)
                    {
                        isGrounded = true;
                        break;
                    }
                }
                else
                {
                    // Fallback to bounds-based approximation
                    Debug.Log("Raycast failed; checking using collider bounds.");
                    if (collider is BoxCollider || collider is MeshCollider)
                    {
                        Vector3 surfaceNormal = Vector3.up; // Assume flat surface
                        if (Vector3.Angle(surfaceNormal, Vector3.up) < 45f)
                        {
                            isGrounded = true;
                            break;
                        }
                    }
                }
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                Instantiate(jumpParticlesPrefab, groundCheckTransform.position - new Vector3(0, 0.15f, 0), Quaternion.Euler(-90, 0, 0));
            }
            else
            {
                Debug.LogWarning("Jump input detected, but player is not grounded.");

                if (colliders.Length == 0)
                {
                    Debug.LogWarning("No colliders detected within the ground check sphere.");
                }
                else
                {
                    foreach (Collider collider in colliders)
                    {
                        Bounds colliderBounds = collider.bounds;

                        if (groundCheckPos.y <= colliderBounds.max.y - 0.1f)
                        {
                            Debug.Log("Ground check position is below or inside the collider bounds.");
                        }
                        else
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(groundCheckPos, Vector3.down, out hit, sphereRadius + 0.1f))
                            {
                                if (Vector3.Angle(hit.normal, Vector3.up) >= 45f)
                                {
                                    Debug.Log($"Surface detected but too steep: {Vector3.Angle(hit.normal, Vector3.up)} degrees.");
                                }
                            }
                            else
                            {
                                Debug.Log("Raycast did not hit any valid ground surface.");
                            }
                        }
                    }
                }

                Debug.Log($"Ground check position: {groundCheckPos}, sphere radius: {sphereRadius}");
                Debug.Log($"Number of colliders detected: {colliders.Length}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheckTransform.position, 0.23f);
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }
}
