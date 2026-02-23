using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlathformerPlayer : MonoBehaviour
{
    [Header("Player ID")]
    [SerializeField] private int playerId;

    [Header("Movement")]
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpHoldForce = 35f;
    [SerializeField] private float maxJumpHoldTime = 0.5f;
    [SerializeField] private float earlyReleaseTime = 0.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Better Jump Physics")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 4f;

    [Header("References")]
    [SerializeField] private Transform body;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform savePoint;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 velocity;

    public bool canMove = true;

    private bool isGrounded;
    private bool jumpHeld;
    private float jumpHoldTimer;
    private float jumpPressTimer;

    private void OnEnable()
    {
        PlayerInputProperties p = PlayerJoinManager.Instance.playerInputSet(playerId);
        p.OnMoveInput += OnMove;
        p.OnSouthPressed += OnJumpPressed;
        p.OnSouthReleased += OnJumpReleased;
    }

    private void OnDisable()
    {
        PlayerInputProperties p = PlayerJoinManager.Instance.playerInputSet(playerId);
        p.OnMoveInput -= OnMove;
        p.OnSouthPressed -= OnJumpPressed;
        p.OnSouthReleased -= OnJumpReleased;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    void OnJumpPressed()
    {
        if (!canMove || !isGrounded) return;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpHeld = true;
        jumpHoldTimer = maxJumpHoldTime;
        jumpPressTimer = 0f;
    }

    void OnJumpReleased()
    {
        jumpHeld = false;
    }

    void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }

    void ApplyBetterJump()
    {
        if (jumpHeld && jumpHoldTimer > 0f && rb.linearVelocity.y > 0f)
        {
            rb.AddForce(Vector3.up * jumpHoldForce, ForceMode.Acceleration);
            jumpHoldTimer -= Time.fixedDeltaTime;
            jumpPressTimer += Time.fixedDeltaTime;
        }

        if (!jumpHeld && jumpPressTimer < earlyReleaseTime && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity += Vector3.up *
                Physics.gravity.y *
                (lowJumpMultiplier - 1f) *
                Time.fixedDeltaTime;
        }

        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector3.up *
                Physics.gravity.y *
                (fallMultiplier - 1f) *
                Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            transform.position = savePoint.position;
        }
        else if (other.CompareTag("car"))
        {
            transform.position = savePoint.position;
        }
        else if (other.CompareTag("side"))
        {
            other.gameObject.SetActive(false);
            plathformerManager.Instance._SpawnCoin();
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        CheckGround();
        ApplyBetterJump();

        float dt = Time.fixedDeltaTime;

        Vector3 targetVelocity =
            new Vector3(moveInput.x, 0f, moveInput.y) * maxSpeed;

        if (moveInput.sqrMagnitude > 0.01f && isGrounded)
        {
            if (anim) anim.SetInteger("walk", 1);
        }
        else
        {
            if (anim) anim.SetInteger("walk", 0);
        }

        velocity = Vector3.MoveTowards(
            velocity,
            targetVelocity,
            acceleration * dt
        );

        Vector3 move = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        rb.linearVelocity = move;

        Vector3 lookDir = new Vector3(moveInput.x, 0, moveInput.y);

        if (lookDir.sqrMagnitude > 0.001f && body != null)
        {
            Quaternion targetRot =
                Quaternion.LookRotation(lookDir.normalized);

            body.rotation = Quaternion.Slerp(
                body.rotation,
                targetRot,
                rotationSpeed * dt
            );
        }
    }
}