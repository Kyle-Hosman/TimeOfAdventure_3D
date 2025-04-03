using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterController characterController; // Reference to the CharacterController component
    public Camera playerCamera; // Reference to the camera
    public float walkSpeed = 5f; // Speed of the player
    public float runSpeed = 10f; // Speed of the player when running
    public float jumpSpeed = 8f; // Speed of the player when jumping
    public float gravity = 20f; // Gravity applied to the player
    public float lookSpeed = 2f; // Speed of the camera rotation
    public float lookXLimit = 45f; // Limit for the camera rotation on the X axis

    Vector3 moveDirection = Vector3.zero; // Direction of the player's movement
    public bool canMove = true; // Flag to check if the player can move
    float rotationX = 0; // Rotation of the camera on the X axis
    CharacterController controller; // Reference to the CharacterController component

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterController controller = GetComponent<CharacterController>(); // Get the CharacterController component
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Confined; // Lock the cursor to the game window
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = playerCamera.transform.TransformDirection(Vector3.forward); // Get the forward direction of the camera
        Vector3 right = playerCamera.transform.TransformDirection(Vector3.right); // Get the right direction of the camera

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Check if the Left Shift key is pressed
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0; // Set the current speed based on whether the player can move and if they are running
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0; // Set the current speed based on whether the player can move and if they are running
        float moveDirectionY = moveDirection.y; // Store the current Y direction of the movement
        moveDirection = (forward * curSpeedX) + (right * curSpeedY); // Calculate the movement direction based on the forward and right directions of the camera

        // Press Space to jump
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded) // Check if the Jump button is pressed and if the player can move
        {
            moveDirection.y = jumpSpeed; // Set the Y direction of the movement to the jump speed
        }
        else
        {
            moveDirection.y = moveDirectionY; // Keep the Y direction of the movement unchanged
        }

        if (!characterController.isGrounded) // Check if the player is not grounded
        {
            moveDirection.y -= gravity * Time.deltaTime; // Apply gravity to the Y direction of the movement
        }

        // Handle rotation
        characterController.Move(moveDirection * Time.deltaTime); // Move the character controller based on the movement direction and delta time

        if (canMove) {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed; // Rotate the camera on the X axis based on the mouse movement
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // Clamp the rotation to the specified limit
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Set the local rotation of the camera based on the clamped rotation
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); // Rotate the player based on the mouse movement
        }

    }
}
