using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 10f;
    public float gravity = 20f;
    public float airControl = 0.5f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool jumpRequested;
    private float lastJumpTime;
    private const float jumpGroundingPreventionTime = 0.2f;

    private Vector3 moveDirection;
    private Vector3 inputDirection;
    private bool isRunning;
    private bool movementDisabled;
    private Animator animator;
    private bool isSprinting;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>(); // Ensure this finds the Animator

        // Lock the mouse cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onSprintPressed += StartSprinting;
        GameEventsManager.instance.inputEvents.onSprintReleased += StopSprinting;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onSprintPressed -= StartSprinting;
        GameEventsManager.instance.inputEvents.onSprintReleased -= StopSprinting;
    }

    private void Update()
    {
        if (movementDisabled) return; // Prevent movement if disabled

        // Handle movement, jumping, sprinting, and attacking
        HandleMovement();
        HandleJump();
        HandleSprinting();
        HandleAttacking();

        // Update Animator parameters
        UpdateAnimator();
    }

    private void HandleMovement()
    {
        // Check if the player is grounded
        GroundCheck();

        // Get input direction
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        inputDirection = new Vector3(inputX, 0, inputZ).normalized;

        // Get the camera's forward and right directions
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = cameraTransform.right;

        // Calculate movement direction relative to the camera
        moveDirection = (cameraForward * inputZ + cameraRight * inputX).normalized;

        // Adjust movement speed based on sprinting
        float currentSpeed = isSprinting ? runSpeed : walkSpeed;
        moveDirection *= isGrounded ? currentSpeed : currentSpeed * airControl;

        // Rotate the player to face the camera's forward direction only when moving forward/backward
        if (inputZ != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Apply movement
        if (isGrounded)
        {
            velocity.x = moveDirection.x;
            velocity.z = moveDirection.z;
        }
        else
        {
            // Allow limited air control
            velocity.x = Mathf.Lerp(velocity.x, moveDirection.x, airControl * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, moveDirection.z, airControl * Time.deltaTime);
        }

        // Apply gravity
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        // Move the character
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        // Check for jump input
        if (Input.GetKeyDown(KeyCode.Space) && CanJump())
        {
            Debug.Log("Jump requested.");
            jumpRequested = true;
            lastJumpTime = Time.time;
            velocity.y = jumpForce; // Apply upward velocity
            animator.SetBool("IsJumping", true); // Trigger jump animation
        }

        // Reset jump request after leaving the ground
        if (isGrounded && velocity.y <= 0)
        {
            jumpRequested = false;
            animator.SetBool("IsJumping", false); // Reset jump animation
        }
    }

    private void HandleSprinting()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
            moveDirection *= runSpeed / walkSpeed; // Increase speed
        }
        else
        {
            isSprinting = false;
        }
    }

    private void HandleAttacking()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            animator.SetTrigger("Attack"); // Trigger attack animation
        }
    }

    private void GroundCheck()
    {
        // Prevent snapping to the ground immediately after jumping
        float checkDistance = isGrounded ? groundCheckDistance : 0.2f; // Increased distance for better detection

        // Calculate capsule positions
        Vector3 capsuleBottom = transform.position + controller.center - Vector3.up * (controller.height / 2f - controller.radius);
        Vector3 capsuleTop = transform.position + controller.center + Vector3.up * (controller.height / 2f - controller.radius);

        // Perform a capsule cast to check for ground
        isGrounded = Physics.CapsuleCast(capsuleBottom, capsuleTop, controller.radius, Vector3.down, out RaycastHit hit, checkDistance, groundLayer);

        // Debug the capsule cast
        Debug.DrawLine(capsuleBottom, capsuleBottom + Vector3.down * checkDistance, isGrounded ? Color.green : Color.red);

        // Prevent grounding immediately after jumping
        if (Time.time < lastJumpTime + jumpGroundingPreventionTime)
        {
            isGrounded = false;
        }
    }

    private bool CanJump()
    {
        return isGrounded && !jumpRequested;
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            // Update grounded state
            animator.SetBool("IsGrounded", isGrounded);

            // Update movement parameters
            Vector3 localVelocity = transform.InverseTransformDirection(controller.velocity);

            // Determine the correct max speed based on whether the player is sprinting
            float maxSpeed = isSprinting ? runSpeed : walkSpeed;

            // Normalize Velocity X and Z based on the current speed (walk or run)
            float normalizedVelocityX = Mathf.Clamp(localVelocity.x / runSpeed, -1f, 1f); // Always normalize to runSpeed for blend tree
            float normalizedVelocityZ = Mathf.Clamp(localVelocity.z / runSpeed, -1f, 1f); // Always normalize to runSpeed for blend tree

            // Scale the values for walking if not sprinting
            if (!isSprinting)
            {
                normalizedVelocityX *= walkSpeed / runSpeed; // Scale down to walking range
                normalizedVelocityZ *= walkSpeed / runSpeed; // Scale down to walking range
            }

            // Set the animator parameters
            animator.SetFloat("Velocity X", normalizedVelocityX);
            animator.SetFloat("Velocity Z", normalizedVelocityZ);

            // Adjust animation speed based on state
            if (animator.GetBool("IsJumping"))
            {
                animator.speed = 0.8f; // Slow down the jump animation
            }
            else if (isSprinting)
            {
                animator.speed = Mathf.Clamp(localVelocity.magnitude / runSpeed, 1.2f, 1.5f); // Slightly adjust for running
            }
            else
            {
                animator.speed = Mathf.Clamp(localVelocity.magnitude / walkSpeed, 0.8f, 1.2f); // Slightly adjust for walking
            }
        }
    }

    private void StartSprinting()
    {
        isSprinting = true;
    }

    private void StopSprinting()
    {
        isSprinting = false;
    }
}
