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

    private void OnEnable()
    {
        // Subscribe to InputEvents
        GameEventsManager.instance.inputEvents.onMovePressed += OnMovePressed;
        GameEventsManager.instance.inputEvents.onJumpPressed += OnJumpPressed;

        // Subscribe to PlayerEvents
        GameEventsManager.instance.playerEvents.onDisablePlayerMovement += DisablePlayerMovement;
        GameEventsManager.instance.playerEvents.onEnablePlayerMovement += EnablePlayerMovement;
    }

    private void OnDisable()
    {
        // Unsubscribe from InputEvents
        GameEventsManager.instance.inputEvents.onMovePressed -= OnMovePressed;
        GameEventsManager.instance.inputEvents.onJumpPressed -= OnJumpPressed;

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
        moveDirection.x = desiredMoveDirection.x * speed;
        moveDirection.z = desiredMoveDirection.z * speed;

        // Handle jumping and gravity
        if (controller.isGrounded)
        {
            if (jumpRequested)
            {
                moveDirection.y = jumpSpeed; // Apply jump speed
                jumpRequested = false; // Reset jump request
                Debug.Log($"Jump applied! moveDirection.y = {moveDirection.y}"); // Debug log
            }
            else
            {
                moveDirection.y = 0; // Reset Y direction when grounded
            }
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }

        // Update the Animator parameters
        UpdateAnimator();
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
            isRunning = Input.GetKey(KeyCode.LeftShift); // Check if the Left Shift key is pressed
        }
    }

    private void OnJumpPressed()
    {
        if (controller.isGrounded && !movementDisabled)
        {
            jumpRequested = true; // Set jump request
            Debug.Log("Jump requested!"); // Debug log for jump request
        }
    }

    private void DisablePlayerMovement()
    {
        movementDisabled = true;
        inputDirection = Vector3.zero; // Stop movement immediately
        moveDirection = Vector3.zero; // Reset move direction
                                      // Override Cinemachine input axes
        //CinemachineCore.GetInputAxis = (axisName) => 0;
        cinemachineCamera.gameObject.SetActive(false);
        Debug.Log("Player movement disabled.");
    }

    private void EnablePlayerMovement()
    {
        movementDisabled = false;
        // Restore Cinemachine input axes
        //CinemachineCore.GetInputAxis = UnityEngine.Input.GetAxis;
        cinemachineCamera.gameObject.SetActive(true);
        Debug.Log("Player movement enabled.");
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

        // Set the Velocity X and Velocity Z parameters in the Animator
        animator.SetFloat("Velocity X", scaledX); // Sideways movement
        animator.SetFloat("Velocity Z", scaledZ); // Forward/backward movement

        // Set the VerticalVelocity parameter for jumping
        animator.SetFloat("Vertical Velocity", moveDirection.y);
    }
}
}
