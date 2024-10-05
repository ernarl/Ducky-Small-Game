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

        isGrounded = Physics.CheckSphere(groundCheckTransform.position, 0.23f, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Instantiate(jumpParticlesPrefab, groundCheckTransform.position - new Vector3(0, 0.15f, 0), Quaternion.Euler(-90, 0, 0));
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
