using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerControllers : MonoBehaviour
    {
        [Header("References")]
        public GameManager gameManager;
        public CharacterController characterController;

        public PlayerManager player;

        [Header("Player Attribute Settings")]
        private float playerMass = 70f; // Player weight in kg
        private float airDrag = 0.1f; // Resistance when falling

        [Header("Movement Settings")]
        public float walkSpeed = 5f;
        public float jumpHeight = 3.5f;

        [Header("Shooting Settings")]
        public GameObject bulletPrefab;
        public float bulletSpeed = 20f;
        public float offsetAim = 0.8f;
        public float offsetShoot = 1.0f;

        [Header("Slam Settings")]
        private float slamSpeed = -40f; // How fast you slam down

        public float airControl = 6f;


        [Header("Input")]
        private float horizontalInput;
        private float verticalInput;
        private bool jumpInput;

        private Vector3 velocity;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
           
        }

        // Update is called once per frame
        private void Update()
        {
            if (player.IsDead())
            {
                // Disable player movement if dead
                return;
            }
            HandleMovementInput();
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
            float currentSpeed = walkSpeed;

            // Input in local space (camera-relative movement)
            Vector3 inputDir = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            inputDir = transform.TransformDirection(inputDir);

            if (characterController.isGrounded)
            {
                // On ground: full control + velocity matches input
                velocity.x = inputDir.x * currentSpeed;
                velocity.z = inputDir.z * currentSpeed;
            }
            else
            {
                // In air: limited control
                Vector3 targetVelocity = inputDir * currentSpeed;
                velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, airControl * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, airControl * Time.deltaTime);
            }
        }


        private void JumpingManagement()
        {
            if (jumpInput && characterController.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gameManager.GetGravity());
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
                // Apply gravity with mass factor
                velocity.y += gameManager.GetGravity() * Time.deltaTime;

                // Apply air resistance
                float dragEffect = airDrag / (playerMass / 70f);
                velocity.y *= 1f - dragEffect * Time.deltaTime;
            }
        }

        // Handle movement input (WASD)
        private void HandleMovementInput()
        {
            // If both A and D are pressed, cancel out horizontal movement
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
            {
                horizontalInput = 0f;
            }
            else
            {
                horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D for strafing left/right
            }

            // If both W and S are pressed, cancel out vertical movement
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
            {
                verticalInput = 0f;
            }
            else
            {
                verticalInput = Input.GetAxisRaw("Vertical"); // W/S for forward/backward
            }

            // Space for jumping
            jumpInput = Input.GetButtonDown("Jump");

            // Left Shift for slide
        }

        public void Shoot()
        {
            // Get camera direction for aiming
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;

            Vector3 shootDirection = mainCamera.transform.forward;

            // Spawn bullet at right hand position, moved forward to avoid body
            Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.right * offsetAim + shootDirection * offsetShoot;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(shootDirection));

            // Add velocity to bullet in camera direction
            if (bullet.TryGetComponent<Rigidbody>(out var bulletRb))
            {
                // Configure Rigidbody for smooth physics with minimal performance cost
                bulletRb.interpolation = RigidbodyInterpolation.Interpolate;
                bulletRb.collisionDetectionMode = CollisionDetectionMode.Discrete;

                // Set velocity in camera direction
                bulletRb.linearVelocity = shootDirection * bulletSpeed;

                // Ignore collision with player to prevent immediate hits
                if (TryGetComponent<Collider>(out var playerCollider))
                {
                    if (bullet.TryGetComponent<Collider>(out var bulletCollider))
                    {
                        Physics.IgnoreCollision(playerCollider, bulletCollider);
                    }
                }
            }
        }
        public void Slam()
        {
            velocity.x = 0f;
            velocity.z = 0f;
            velocity.y = slamSpeed;
        }


    }
}
