using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        private CharacterController characterController;

        private PlayerManager player;

        [Header("Player Attribute Settings")]
        private readonly float playerMass = 70f; // Player weight in kg
        private readonly float airDrag = 0.1f; // Resistance when falling

        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float jumpHeight = 3.5f;

        [Header("Shooting Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed = 20f;
        private readonly float offsetAim = 0.8f;
        private readonly float offsetShoot = 1.0f;

        [Header("Input")]
        private float horizontalInput;
        private float verticalInput;
        private bool jumpInput;
        private bool runInput;

        private Vector3 velocity;

        [Header("Effect Settings")]
        private bool isSlowed = false;
        private float slowEndTime = 0f;
        private float speedMultiplier = 1f; // Normal speed multiplier

        // Dash effect variables
        private bool isDashing = false;
        private float dashEndTime = 0f;
        private Vector3 dashDirection;
        private float dashSpeed = 0f;
        [SerializeField] private float dashDuration = 0.3f; // Duration of dash in seconds

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

            // Handle shooting
            ShootingManagement();

            // Handle apply card effects
            ApplyEffectManagement();

            // Move the character controller
            characterController.Move(velocity * Time.deltaTime);
        }

        // Handle walking, strafing, and running movement
        private void MovementAndRunningManagement()
        {
            // Update slow effect status
            UpdateSlowEffect();

            // Update dash effect status
            UpdateDashEffect();

            // If dashing, override normal movement
            if (isDashing)
            {
                velocity.x = dashDirection.x * dashSpeed;
                velocity.z = dashDirection.z * dashSpeed;
            }
            else
            {
                // Calculate movement speed (walk or run)
                float currentSpeed = runInput ? runSpeed : walkSpeed;

                // Apply speed multiplier (for slow effect)
                currentSpeed *= speedMultiplier;

                // W/S for forward/backward, A/D for left/right strafing
                Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized * currentSpeed;

                // Apply horizontal movement (keep y velocity separate for jumping/gravity)
                velocity.x = moveDirection.x;
                velocity.z = moveDirection.z;
            }
        }

        // Handle jumping movement
        private void JumpingManagement()
        {
            if (jumpInput && characterController.isGrounded)
            {
                // Apply jump velocity using physics formula
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

        // Handle player shooting
        private void ShootingManagement()
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

        // Handle player apply effect
        private void ApplyEffectManagement()
        {
            if (Input.GetMouseButtonDown(1))
            {
                CardData currentCard = player.GetCurrentCard();

                if (currentCard != null)
                {
                    switch (currentCard.effectType)
                    {
                        case EffectType.Heal:
                            ApplyHealingEffect(currentCard);
                            break;
                        case EffectType.Dash:
                            ApplyDashingEffect(currentCard);
                            break;
                        case EffectType.Slow:
                            ApplySlowEffect(currentCard);
                            break;
                    }

                    // Clear the card after using it
                    player.ClearCurrentCard();
                }
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

        // Apply healing effect from card
        private void ApplyHealingEffect(CardData card)
        {
            if (card != null)
            {
                player.Heal((int)card.value);
                Debug.Log($"Applied {card.cardName}: Healed {card.value} HP");
            }
        }

        // Apply dash effect - fast forward movement
        private void ApplyDashingEffect(CardData card)
        {
            if (card != null)
            {
                // Get the forward direction (camera forward for better control)
                Camera mainCamera = Camera.main;

                if (mainCamera != null)
                {
                    // Use camera forward direction but keep it horizontal
                    dashDirection = mainCamera.transform.forward;
                    dashDirection.y = 0; // Keep dash horizontal
                    dashDirection.Normalize();
                }
                else
                {
                    // Fallback to player forward direction
                    dashDirection = transform.forward;
                }

                // Calculate dash speed needed to cover the distance in dashDuration
                float dashDistance = card.value;
                dashSpeed = dashDistance / dashDuration;

                // Start dashing
                isDashing = true;
                dashEndTime = Time.time + dashDuration;
            }
        }

        // Update dash effect timer
        private void UpdateDashEffect()
        {
            if (isDashing && Time.time >= dashEndTime)
            {
                isDashing = false;
                dashSpeed = 0f;
            }
        }

        // Apply slow effect - reduce speed to 50% for duration
        private void ApplySlowEffect(CardData card)
        {
            if (card != null)
            {
                // Apply slow effect
                isSlowed = true;
                speedMultiplier = 0.5f; // 50% speed
                slowEndTime = Time.time + card.value; // Duration in seconds

                Debug.Log($"Applied {card.cardName}: Speed reduced to 50% for {card.value} seconds");
            }
        }

        // Update slow effect timer
        private void UpdateSlowEffect()
        {
            if (isSlowed && Time.time >= slowEndTime)
            {
                // Slow effect has ended, restore normal speed
                isSlowed = false;
                speedMultiplier = 1f;
                Debug.Log("Slow effect ended - speed restored to normal");
            }
        }
    }
}
