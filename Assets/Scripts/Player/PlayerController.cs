using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterController characterController; // Reference to the CharacterController component
    public Camera playerCamera; // Reference to the camera
    public float walkSpeed = 0.5f; // Speed of the player
    public float runSpeed = 2f; // Speed of the player when running
    public float jumpSpeed = 8f; // Speed of the player when jumping
    public float gravity = 20f; // Gravity applied to the player
    public float lookSpeed = 2f; // Speed of the camera rotation
    public float lookXLimit = 45f; // Limit for the camera rotation on the X axis

    Vector3 moveDirection = Vector3.zero; // Direction of the player's movement
    public bool canMove = true; // Flag to check if the player can move
    float rotationX = 0; // Rotation of the camera on the X axis
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
        Vector3 forward = playerCamera.transform.TransformDirection(Vector3.forward); // Get the forward direction of the camera
        Vector3 right = playerCamera.transform.TransformDirection(Vector3.right); // Get the right direction of the camera

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Check if the Left Shift key is pressed
        float speed = isRunning ? runSpeed : walkSpeed; // Use runSpeed if running, otherwise walkSpeed

        // Calculate movement direction based on input and speed
        float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0; // Forward/backward movement
        float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0; // Sideways movement
        float moveDirectionY = moveDirection.y; // Store the current Y direction of the movement
        moveDirection = (forward * curSpeedX) + (right * curSpeedY); // Calculate the movement direction based on the forward and right directions of the camera
        moveDirection.y = moveDirectionY; // Preserve the Y direction of the movement

        // Press Space to jump
        if (Input.GetButton("Jump") && canMove && controller.isGrounded)
        {
            moveDirection.y = jumpSpeed; // Set the Y direction of the movement to the jump speed
        }
        else if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime; // Apply gravity to the Y direction of the movement
        }

        // Move the character controller
        controller.Move(moveDirection * Time.deltaTime);

        // Handle rotation
        if (canMove)
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed; // Rotate the camera on the X axis based on the mouse movement
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // Clamp the rotation to the specified limit
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Set the local rotation of the camera based on the clamped rotation
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); // Rotate the player based on the mouse movement
        }

        // Update the Animator parameters
        UpdateAnimator();
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
            scaledZ = Mathf.Clamp(scaledZ, 0f, 2f);  // 0 to 2 for forward/backward movement

            // Set the Velocity X and Velocity Z parameters in the Animator
            animator.SetFloat("Velocity X", scaledX); // Sideways movement
            animator.SetFloat("Velocity Z", scaledZ); // Forward/backward movement
        }
    }
}
