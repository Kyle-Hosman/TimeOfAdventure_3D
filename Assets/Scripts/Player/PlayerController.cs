using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f; // Now represents mid-speed running
    public float sprintSpeed = 12f; // New, fastest speed
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
    private bool isRunning; // True if running (mid-speed), false if walking
    private bool isSprinting; // True if sprinting (fastest)
    private bool movementDisabled;
    private Animator animator;
    private bool jumpForceApplied = false;

    private float currentSpeed; // Add a field to track the current movement speed
    private const float speedTransitionTime = 0.5f; // Time to transition between walk and sprint speeds

    private bool walkToggle = true; // true = walk, false = run (default to walk)

    public GameObject swordObject; // Assign in Inspector
    private bool swordEquipped = true;

    private int attackIndex = 0; // 0 = inward, 1 = outward

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>(); // Ensure this finds the Animator
        currentSpeed = walkSpeed; // Initialize current speed to walking speed

        // Lock the mouse cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onSprintPressed += StartSprinting;
        GameEventsManager.instance.inputEvents.onSprintReleased += StopSprinting;
        GameEventsManager.instance.inputEvents.onRunPressed += StartRunning;
        GameEventsManager.instance.inputEvents.onRunReleased += StopRunning;
        GameEventsManager.instance.inputEvents.onJumpPressed += OnJumpPressed;
        GameEventsManager.instance.inputEvents.onAttackPressed += OnAttackPressed;
        GameEventsManager.instance.inputEvents.onWalkTogglePressed += OnWalkTogglePressed; 
        GameEventsManager.instance.playerEvents.onDisablePlayerMovement += DisablePlayerMovement;
        GameEventsManager.instance.playerEvents.onEnablePlayerMovement += EnablePlayerMovement;
        GameEventsManager.instance.inputEvents.onPreviousPressed += ToggleSword;
        GameEventsManager.instance.inputEvents.onBlockPressed += OnBlockPressed;
        GameEventsManager.instance.inputEvents.onBlockReleased += OnBlockReleased;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onSprintPressed -= StartSprinting;
        GameEventsManager.instance.inputEvents.onSprintReleased -= StopSprinting;
        GameEventsManager.instance.inputEvents.onRunPressed -= StartRunning;
        GameEventsManager.instance.inputEvents.onRunReleased -= StopRunning;
        GameEventsManager.instance.inputEvents.onJumpPressed -= OnJumpPressed;
        GameEventsManager.instance.inputEvents.onAttackPressed -= OnAttackPressed;
        GameEventsManager.instance.inputEvents.onWalkTogglePressed -= OnWalkTogglePressed;
        GameEventsManager.instance.playerEvents.onDisablePlayerMovement -= DisablePlayerMovement;
        GameEventsManager.instance.playerEvents.onEnablePlayerMovement -= EnablePlayerMovement;
        GameEventsManager.instance.inputEvents.onPreviousPressed -= ToggleSword;
        GameEventsManager.instance.inputEvents.onBlockPressed -= OnBlockPressed;
        GameEventsManager.instance.inputEvents.onBlockReleased -= OnBlockReleased;
    }

    private void Update()
    {
        if (movementDisabled) return;
        HandleMovement();
        HandleJump();
        HandleRunning();
        HandleSprinting();
        HandleAttacking();
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

        // Determine speed
        float targetSpeed = walkSpeed;
        if (isSprinting)
            targetSpeed = sprintSpeed;
        else if (isRunning)
            targetSpeed = runSpeed;
        // else walking
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / speedTransitionTime);
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

    private void HandleRunning()
    {
        // No direct input checks here. State is set by event handlers.
        // walkToggle determines if we are walking or running (unless sprinting)
        if (!isSprinting)
        {
            isRunning = !walkToggle;
        }
    }

    private void StartRunning()
    {
        isRunning = true;
    }

    private void StopRunning()
    {
        isRunning = false;
    }

    private void OnJumpPressed()
    {
        if (CanJump())
        {
            Debug.Log("Jump requested.");
            jumpRequested = true;
            lastJumpTime = Time.time;

            if (isSprinting)
            {
                animator.SetTrigger("Running_Jump");
                Debug.Log("Running_Jump animation triggered.");
            }
            else
            {
                animator.SetTrigger("Jump");
                Debug.Log("Jump animation triggered.");
            }

            velocity.y = 0.1f;
        }
    }

    private void HandleJump()
    {
        if (isGrounded && velocity.y <= 0)
        {
            jumpRequested = false;
            jumpForceApplied = false;

            animator.ResetTrigger("Running_Jump");
            animator.ResetTrigger("Jump");
        }
    }

    private void HandleSprinting()
    {
        // If you use input events for sprinting, update this logic to set isSprinting only
        // and let HandleRunning handle the mid-speed run
    }

    private void OnAttackPressed()
    {
        if (movementDisabled) return; // Prevent attack if movement is disabled
        if (swordEquipped && swordObject != null)
        {
            if (animator != null)
            {
                if (attackIndex == 0)
                {
                    animator.SetTrigger("Attack_Inward");
                }
                else
                {
                    animator.SetTrigger("Attack_Outward");
                }
                attackIndex = 1 - attackIndex; // Toggle between 0 and 1
            }
        }
        
    }

    private void OnBlockPressed()
    {
        if (movementDisabled) return; // Prevent blocking if movement is disabled
        if (swordEquipped && swordObject != null)
        {
            if (animator != null)
            {
                animator.SetBool("IsBlocking", true);
            }
        }
    }

    private void OnBlockReleased()
    {
        if (animator != null)
        {
            animator.SetBool("IsBlocking", false);
        }
    }


    private void DisablePlayerMovement()
    {
        movementDisabled = true;
    }

    private void EnablePlayerMovement()
    {
        movementDisabled = false;
    }

    private void OnWalkTogglePressed()
    {
        walkToggle = !walkToggle;
        Debug.Log($"WalkToggle is now {(walkToggle ? "WALK" : "RUN")}");
    }

    private void HandleAttacking()
    {
        // if (Input.GetMouseButtonDown(0)) // Left mouse button
        // {
        //     animator.SetTrigger("Attack"); // Trigger attack animation
        // }
    }

    private void GroundCheck()
    {
        // Prevent snapping to the ground immediately after jumping
        float checkDistance = isGrounded ? groundCheckDistance : 0.2f; // Increased distance for better detection

        // Calculate capsule positions based on the updated collider size
        Vector3 capsuleBottom = transform.position + controller.center - Vector3.up * (controller.height / 2f - controller.radius * 0.9f); // Adjusted for larger character
        Vector3 capsuleTop = transform.position + controller.center + Vector3.up * (controller.height / 2f - controller.radius * 0.9f); // Adjusted for larger character

        // Perform a capsule cast to check for ground
        isGrounded = Physics.CapsuleCast(capsuleBottom, capsuleTop, controller.radius, Vector3.down, out RaycastHit hit, checkDistance, groundLayer);

        // Prevent grounding immediately after jumping
        if (Time.time < lastJumpTime + jumpGroundingPreventionTime)
        {
            isGrounded = false;
        }

        // Debug the capsule cast
        Debug.DrawLine(capsuleBottom, capsuleBottom + Vector3.down * checkDistance, isGrounded ? Color.green : Color.red);
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
            float maxSpeed = walkSpeed;
            if (isSprinting)
                maxSpeed = sprintSpeed;
            else if (isRunning)
                maxSpeed = runSpeed;

            // Normalize Velocity X and Z based on the current speed (walk or run)
            float normalizedVelocityX = Mathf.Clamp(localVelocity.x / maxSpeed, -1f, 1f); // Always normalize to runSpeed for blend tree
            float normalizedVelocityZ = Mathf.Clamp(localVelocity.z / maxSpeed, -1f, 1f); // Always normalize to runSpeed for blend tree

            // Scale for blend tree: walk = 0.5, run = 1, sprint = 1.5
            float walkScale = 0.5f;
            float runScale = 1.0f;
            float sprintScale = 1.5f;
            if (isSprinting)
            {
                normalizedVelocityX *= sprintScale;
                normalizedVelocityZ *= sprintScale;
            }
            else if (isRunning)
            {
                normalizedVelocityX *= runScale;
                normalizedVelocityZ *= runScale;
            }
            else // walking
            {
                normalizedVelocityX *= walkScale;
                normalizedVelocityZ *= walkScale;
            }

            // Set the animator parameters
            animator.SetFloat("Velocity X", normalizedVelocityX);
            animator.SetFloat("Velocity Z", normalizedVelocityZ);

            // Optionally set animator bools for running/sprinting
            //animator.SetBool("IsRunning", isRunning);
            //animator.SetBool("IsSprinting", isSprinting);

            // Adjust animation speed based on state
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump")) // Check if in jump state
            {
                animator.speed = 1.5f; // Adjust speed for jumping
            }
            else if (isSprinting)
            {
                animator.speed = Mathf.Clamp(localVelocity.magnitude / sprintSpeed, 1.5f, 0.5f); // Slightly adjust for running
            }
            else if (isRunning)
            {
                animator.speed = Mathf.Clamp(localVelocity.magnitude / runSpeed, 1.2f, 1.5f); // Slightly adjust for running
            }
            else
            {
                animator.speed = Mathf.Clamp(localVelocity.magnitude / walkSpeed, 1.0f, 1.2f); // Slightly adjust for walking
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

    // This method will be called by the animation event
    public void ApplyJumpForceFromEvent()
    {
        if (jumpForceApplied) return; // Prevent multiple calls

        Debug.Log($"ApplyJumpForce called at frame: {animator.GetCurrentAnimatorStateInfo(0).normalizedTime}");
        velocity.y = jumpForce; // Apply upward velocity
        jumpForceApplied = true; // Mark as applied
    }

    private void ToggleSword()
    {
        if (animator != null)
        {
            animator.SetTrigger("Sword_Equip");
        }
    }
    
    public void ToggleSwordObject()
    {
        swordEquipped = !swordEquipped;
        if (swordObject != null)
            swordObject.SetActive(swordEquipped);
    }
}
