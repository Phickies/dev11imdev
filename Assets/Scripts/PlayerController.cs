using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameManager gameManager;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpHeight = 3.5f;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    private bool jumpInput;
    private bool runInput;

    private Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [System.Obsolete]
    private void Start()
    {
        // Initialize character controller reference if not assigned
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        // Find GameManager in the scene if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        InputManagement();
        MovementManagement();
    }

    // Handle player movement
    private void MovementManagement()
    {
        // Handle horizontal and vertical movement
        MovementAndRunningManagement();

        // Handle jumping
        JumpingManagement();

        // Apply gravity
        GravityManagement();

        // Move the character controller
        characterController.Move(velocity * Time.deltaTime);
    }

    // Handle walking, strafing, and running movement
    private void MovementAndRunningManagement()
    {
        // Calculate movement speed (walk or run)
        float currentSpeed = runInput ? runSpeed : walkSpeed;

        // W/S for forward/backward, A/D for left/right strafing
        Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized * currentSpeed;

        // Apply horizontal movement (keep y velocity separate for jumping/gravity)
        velocity.x = moveDirection.x;
        velocity.z = moveDirection.z;
    }

    // Handle jumping movement
    private void JumpingManagement()
    {
        if (jumpInput && characterController.isGrounded)
        {
            // Apply jump velocity using physics formula
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gameManager.gravity);
        }
    }

    // Handle gravity
    private void GravityManagement()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            // Small negative value to keep player grounded
            velocity.y = -2f;
        }
        else
        {
            // Apply gravity
            velocity.y += gameManager.gravity * Time.deltaTime;
        }
    }

    // Handle player input
    private void InputManagement()
    {
        // If both A and D are pressed, cancel out horizontal movement
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            horizontalInput = 0f;
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal"); // A/D for strafing left/right
        }

        // If both W and S are pressed, cancel out vertical movement
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        {
            verticalInput = 0f;
        }
        else
        {
            verticalInput = Input.GetAxis("Vertical"); // W/S for forward/backward
        }

        // Space for jumping
        jumpInput = Input.GetButtonDown("Jump");

        // Left Shift for running
        runInput = Input.GetKey(KeyCode.LeftShift);
    }
}
