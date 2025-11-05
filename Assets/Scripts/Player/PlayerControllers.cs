using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerControllers : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        private CharacterController characterController;

        private PlayerManager player;

        [Header("Player Attribute Settings")]
        private readonly float playerMass = 70f; // Player weight in kg
        private readonly float airDrag = 0.1f; // Resistance when falling

        [Header("Movement Settings")]
        [SerializeField] public float walkSpeed = 5f;
        [SerializeField] public float runSpeed = 10f;
        [SerializeField] public float jumpHeight = 3.5f;

        [Header("Shooting Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed = 20f;
        private readonly float offsetAim = 0.8f;
        private readonly float offsetShoot = 1.0f;

        [Header("Slam Settings")]
        [SerializeField] private float slamSpeed = -40f; // How fast you slam down


        [Header("Input")]
        private float horizontalInput;
        private float verticalInput;
        private bool jumpInput;
        private bool runInput;

        private Vector3 velocity;

        // Jump boost effect variables
        private bool hasJumpBoost = false;
        private float jumpBoostAmount = 0f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
                gameManager = FindFirstObjectByType<GameManager>();
            }

            // Initialize PlayerManager reference if not assigned
            if (player == null)
            {
                // Get all PlayerManager components on this GameObject
                PlayerManager[] managers = GetComponents<PlayerManager>();

                if (managers.Length > 1)
                {
                    Debug.LogWarning($"Found {managers.Length} PlayerManager components! Using the one with infiniteDash enabled.");

                    // Find the one with infiniteDash enabled
                    foreach (var mgr in managers)
                    {
                        if (mgr.infiniteDash)
                        {
                            player = mgr;
                            break;
                        }
                    }

                    // If none found with infiniteDash, use the first one
                    if (player == null)
                    {
                        player = managers[0];
                    }
                }
                else
                {
                    player = GetComponent<PlayerManager>();
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (player.IsDead())
            {
                // Disable player movement if dead
                return;
            }
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
                // Calculate effective jump height with boost if active
                float effectiveJumpHeight = jumpHeight + (hasJumpBoost ? jumpBoostAmount : 0f);

                // Apply jump velocity using physics formula
                velocity.y = Mathf.Sqrt(effectiveJumpHeight * -2f * gameManager.GetGravity());

                // Consume jump boost after one use
                if (hasJumpBoost)
                {
                    hasJumpBoost = false;
                    jumpBoostAmount = 0f;
                }
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

        // Handle player input
        private void InputManagement()
        {
            HandleMovementInput();
            HandleShootingInput();
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

        // Handle card selection input (keys 1-5)

        // Handle player shooting
        private void HandleShootingInput()
        {
            if (Input.GetMouseButtonDown(0) && bulletPrefab != null)
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

        public void WarpTo(Vector3 newPosition)
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();

            characterController.enabled = false;
            transform.position = newPosition;
            characterController.enabled = true;

            velocity = Vector3.zero; // reset gravity 
        }
    }
}
