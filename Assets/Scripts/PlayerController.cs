using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Moving")]
    [SerializeField] private float moveSpeed = 5f; // Speed of the player movement
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float editorFrameRate = 250f; // The editor's frame rate while playing (Game view > Stats), builds move the same as the editor at it

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f; // Force applied to the jump
    [SerializeField] private LayerMask groundLayer; // Layer to identify what is considered ground
    [SerializeField] private Transform groundCheckTransform; // Where the jump particles spawn
    [SerializeField] private float maxGroundAngle = 45f; // Steepest surface that still counts as ground
    [SerializeField] private float coyoteTime = 0.1f; // Can still jump this long after losing contact with the ground
    [SerializeField] private float jumpBufferTime = 0.15f; // A jump pressed this long before landing still happens
    [SerializeField] private float landSoundMinAirTime = 0.3f; // Shorter hops and bumps while rolling don't play the landing sound
    [SerializeField] private float fallGravityMultiplier = 2.5f; // Stronger gravity only on the way down, so falling is quicker but the jump is just as high

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem jumpParticlesPrefab;

    private const float IGNORE_GROUND_AFTER_JUMP_TIME = 0.1f; // Contacts from the jump frame itself shouldn't allow a second jump
    private const float MAX_FRAME_TIME = 0.05f;

    private bool touchingGround;
    private bool groundContactThisStep;
    private float lastGroundedTime = float.NegativeInfinity;
    private float lastJumpPressedTime = float.NegativeInfinity;
    private float lastJumpTime = float.NegativeInfinity;
    private float lastTouchingGroundTime;
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        // So touching the ground the duck starts on doesn't count as landing
        lastTouchingGroundTime = Time.time;
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
        rb.AddForce(moveDirection * moveSpeed * GetFrameRateScale(), ForceMode.Force);

        LimitSpeed();

        if (Input.GetButtonDown("Jump"))
        {
            lastJumpPressedTime = Time.time;
        }

        bool jumpBuffered = Time.time - lastJumpPressedTime <= jumpBufferTime;
        bool isGrounded = Time.time - lastGroundedTime <= coyoteTime;
        if (jumpBuffered && isGrounded)
        {
            Jump();
        }
    }

    // Forces added every frame pile up until the next physics step, so how hard the duck is pushed depends on the frame rate.
    // The movement was tuned in the editor, which runs at a high frame rate, while a build (like WebGL in a browser) runs at a
    // much lower one and the duck barely moved there. So in builds each frame pushes as hard as editorFrameRate frames would
    private float GetFrameRateScale()
    {
        if (Application.isEditor)
        {
            return 1f;
        }

        // Capped, so one slow frame (a hitch) doesn't shove the duck
        return Mathf.Min(Time.deltaTime, MAX_FRAME_TIME) * editorFrameRate;
    }

    private void FixedUpdate()
    {
        // OnCollisionStay isn't called while the rigidbody sleeps, so keep the last result until it wakes up
        if (!rb.IsSleeping())
        {
            touchingGround = groundContactThisStep;
            groundContactThisStep = false;
        }

        if (touchingGround && Time.time - lastJumpTime > IGNORE_GROUND_AFTER_JUMP_TIME)
        {
            lastGroundedTime = Time.time;
        }

        if (IsFalling())
        {
            rb.AddForce(Physics.gravity * (fallGravityMultiplier - 1f), ForceMode.Acceleration);
        }

        if (touchingGround)
        {
            if (Time.time - lastTouchingGroundTime > landSoundMinAirTime)
            {
                AudioManager.Play(SoundNames.Land);
            }
            lastTouchingGroundTime = Time.time;
        }
    }

    // Going up (and moving on the ground) is limited the same as always, so the jump height doesn't change.
    // While falling only the sideways speed is limited, otherwise the fall would be held back to maxSpeed too
    private void LimitSpeed()
    {
        if (IsFalling())
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
            }
        }
        else if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private bool IsFalling()
    {
        return !touchingGround && rb.velocity.y < 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckForGroundContact(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckForGroundContact(collision);
    }

    // Ground is any contact on the ground layer whose surface faces up enough, so walls and the sides of objects don't count
    private void CheckForGroundContact(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.collider.gameObject.layer)) == 0)
        {
            return;
        }

        float minGroundNormalY = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y >= minGroundNormalY)
            {
                groundContactThisStep = true;
                return;
            }
        }
    }

    private void Jump()
    {
        lastJumpPressedTime = float.NegativeInfinity;
        lastGroundedTime = float.NegativeInfinity;
        lastJumpTime = Time.time;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Instantiate(jumpParticlesPrefab, groundCheckTransform.position - new Vector3(0, 0.15f, 0), Quaternion.Euler(-90, 0, 0));
        AudioManager.Play(SoundNames.Jump);
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }
}
