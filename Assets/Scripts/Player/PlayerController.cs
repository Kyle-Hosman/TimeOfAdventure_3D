using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterController characterController; // Reference to the CharacterController component
    public float walkSpeed = 0.5f; // Speed of the player
    public float runSpeed = 2f; // Speed of the player when running
    public float jumpSpeed = 10f; // Speed of the player when jumping
    public float gravity = 20f; // Gravity applied to the player

    private Vector3 moveDirection = Vector3.zero; // Direction of the player's movement
    private Vector3 inputDirection = Vector3.zero; // Input direction from InputEvents
    private bool movementDisabled = false; // Flag to check if movement is disabled
    private bool isRunning = false; // Whether the player is running
    private bool jumpRequested = false; // Whether a jump has been requested
    private CharacterController controller; // Reference to the CharacterController component
    private Animator animator; // Reference to the Animator component
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private float smoothedVerticalVelocity = 0f; // Store the smoothed vertical velocity
    private float smoothingTime = 0.1f; // Adjust this value for smoother transitions

    [Header("Attack Settings")]
    [SerializeField] private GameObject swordPrefab; // Reference to the sword prefab
    [SerializeField] private Transform swordHolder; // Reference to the sword holder (player's hand)
    [SerializeField] private float attackCooldown = 1f; // Cooldown time between attacks

    private bool isAttacking = false; // Whether the player is currently attacking
    private float lastAttackTime = 0f; // Time of the last attack

    private void OnEnable()
    {
        // Subscribe to InputEvents
        GameEventsManager.instance.inputEvents.onMovePressed += OnMovePressed;
        GameEventsManager.instance.inputEvents.onJumpPressed += OnJumpPressed;
        GameEventsManager.instance.inputEvents.onSprintPressed += OnSprintPressed;
        GameEventsManager.instance.inputEvents.onSprintReleased += OnSprintReleased;

        // Subscribe to PlayerEvents
        GameEventsManager.instance.playerEvents.onDisablePlayerMovement += DisablePlayerMovement;
        GameEventsManager.instance.playerEvents.onEnablePlayerMovement += EnablePlayerMovement;
    }

    private void OnDisable()
    {
        // Unsubscribe from InputEvents
        GameEventsManager.instance.inputEvents.onMovePressed -= OnMovePressed;
        GameEventsManager.instance.inputEvents.onJumpPressed -= OnJumpPressed;
        GameEventsManager.instance.inputEvents.onSprintPressed -= OnSprintPressed;
        GameEventsManager.instance.inputEvents.onSprintReleased -= OnSprintReleased;

        // Unsubscribe from PlayerEvents
        GameEventsManager.instance.playerEvents.onDisablePlayerMovement -= DisablePlayerMovement;
        GameEventsManager.instance.playerEvents.onEnablePlayerMovement -= EnablePlayerMovement;
    }

    private void Start()
    {
        // Assign the CharacterController and Animator components
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Hide and lock the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Instantiate the sword and attach it to the sword holder
        if (swordPrefab != null && swordHolder != null)
        {
            Instantiate(swordPrefab, swordHolder);
        }
    }

    private void Update()
    {
        if (movementDisabled) return; // Prevent movement if disabled

        // Disable camera control if the InputEventContext is INVENTORY or DIALOGUE
        if (GameEventsManager.instance.inputEvents.inputEventContext == InputEventContext.INVENTORY ||
            GameEventsManager.instance.inputEvents.inputEventContext == InputEventContext.DIALOGUE)
        {
            return; // Skip the rest of the Update logic, including camera movement
        }

        // Calculate movement direction based on input and camera direction
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten the camera's forward and right vectors to ignore vertical tilt
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate desired movement direction
        Vector3 desiredMoveDirection = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;

        // Apply movement if the player can move
        float speed = isRunning ? runSpeed : walkSpeed;
        //Debug.Log($"Update: isRunning = {isRunning}, speed = {speed}");
        moveDirection.x = desiredMoveDirection.x * speed;
        moveDirection.z = desiredMoveDirection.z * speed;

        // Handle jumping and gravity
        if (controller.isGrounded)
        {
            if (jumpRequested)
            {
                moveDirection.y = jumpSpeed; // Apply jump speed
                Debug.Log($"Jump applied! moveDirection.y = {moveDirection.y}"); // Debug log
            }
            else
            {
                moveDirection.y = 0; // Reset Y direction when grounded
            }

            // Reset jumpRequested only after processing the jump
            jumpRequested = false;
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime; // Apply gravity
        }

        // Move the character controller
        controller.Move(moveDirection * Time.deltaTime);

        // Rotate the player to face the movement direction
        if (desiredMoveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);
            float rotationSpeed = 4f; // Adjust this value to control the smoothness
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Update the Animator parameters
        UpdateAnimator();

        // Handle attack input
        if (Input.GetMouseButtonDown(0) && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    private void OnMovePressed(Vector3 direction)
    {
        if (movementDisabled)
        {
            inputDirection = Vector3.zero; // Prevent movement if disabled
        }
        else
        {
            inputDirection = direction;
        }
    }

    private void OnJumpPressed()
    {
        if (controller.isGrounded && !movementDisabled)
        {
            jumpRequested = true; // Set jump request
        }
    }

    private void DisablePlayerMovement()
    {
        movementDisabled = true;
        inputDirection = Vector3.zero; // Stop movement immediately
        moveDirection = Vector3.zero; // Reset move direction
        cinemachineCamera.gameObject.SetActive(false);
        Debug.Log("Player movement disabled.");
    }

    private void EnablePlayerMovement()
    {
        movementDisabled = false;
        cinemachineCamera.gameObject.SetActive(true);
    }

    private void OnSprintPressed()
    {
        isRunning = true;
    }

    private void OnSprintReleased()
    {
        isRunning = false;
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            // Calculate the player's local movement direction
            Vector3 localVelocity = transform.InverseTransformDirection(controller.velocity);

            // Scale the velocity to match the blend tree values
            float scaleFactor = 2f / runSpeed; // 2 corresponds to running in the blend tree
            float scaledX = localVelocity.x * scaleFactor;
            float scaledZ = localVelocity.z * scaleFactor;

            // Clamp the values to ensure they stay within the blend tree range
            scaledX = Mathf.Clamp(scaledX, -2f, 2f); // -2 to 2 for strafing
            scaledZ = Mathf.Clamp(scaledZ, -2f, 2f);  // 0 to 2 for forward/backward movement

            // Smooth the vertical velocity
            smoothedVerticalVelocity = Mathf.Lerp(smoothedVerticalVelocity, moveDirection.y, Time.deltaTime / smoothingTime);

            // Calculate VelocityDirection based on Vertical Velocity
            float velocityDirection = 0f;
            if (moveDirection.y >= 2f || moveDirection.y <= -2f)
            {
                velocityDirection = 1f; // Jumping
            }
            else if (moveDirection.y <= -2f)
            {
                velocityDirection = 0f; // Not Jumping
            }

            // Set the Animator parameters
            animator.SetFloat("Velocity X", scaledX); // Sideways movement
            animator.SetFloat("Velocity Z", scaledZ); // Forward/backward movement
            animator.SetFloat("Vertical Velocity", smoothedVerticalVelocity); // Smoothed vertical velocity
            animator.SetFloat("VelocityDirection", velocityDirection); // Air speed for jumping
        }
    }

    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Trigger the attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Enable the sword collider (if applicable)
        EnableSwordCollider();

        // Reset the attack state after the animation ends
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        isAttacking = false;

        // Disable the sword collider (if applicable)
        DisableSwordCollider();
    }

    private void EnableSwordCollider()
    {
        Collider swordCollider = swordHolder.GetComponentInChildren<Collider>();
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }
    }

    private void DisableSwordCollider()
    {
        Collider swordCollider = swordHolder.GetComponentInChildren<Collider>();
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
    }
}
