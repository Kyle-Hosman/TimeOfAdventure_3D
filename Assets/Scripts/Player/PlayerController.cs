using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterController characterController; // Reference to the CharacterController component
    public float walkSpeed = 0.5f; // Speed of the player
    public float runSpeed = 2f; // Speed of the player when running
    public float jumpSpeed = 8f; // Speed of the player when jumping
    public float gravity = 50f; // Increased gravity for stronger effect

    Vector3 moveDirection = Vector3.zero; // Direction of the player's movement
    public bool canMove = true; // Flag to check if the player can move
    private CharacterController controller; // Reference to the CharacterController component
    private Animator animator; // Reference to the Animator component

    void Start()
    {
        // Assign the CharacterController and Animator components
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Hide and lock the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get the forward and right directions from the camera
        Transform cameraTransform = Camera.main.transform; // Assumes the main camera is tagged as "MainCamera"
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten the camera's forward and right vectors to ignore vertical tilt
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Check if the Left Shift key is pressed
        float speed = isRunning ? runSpeed : walkSpeed; // Use runSpeed if running, otherwise walkSpeed

        // Calculate movement direction based on input and camera direction
        float inputVertical = Input.GetAxis("Vertical"); // Forward/backward input
        float inputHorizontal = Input.GetAxis("Horizontal"); // Left/right input
        Vector3 desiredMoveDirection = (cameraForward * inputVertical + cameraRight * inputHorizontal).normalized;

        // Apply movement if the player can move
        if (canMove)
        {
            moveDirection.x = desiredMoveDirection.x * speed;
            moveDirection.z = desiredMoveDirection.z * speed;
            // Preserve moveDirection.y for gravity or jumping
        }

        // Apply gravity
        if (controller.isGrounded)
        {
            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpSpeed; // Set the Y direction of the movement to the jump speed
            }
            else
            {
                moveDirection.y = 0; // Reset Y direction when grounded
            }
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

    void FixedUpdate()
    {
        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.fixedDeltaTime; // Apply gravity in FixedUpdate
        }
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
        }
    }
}
