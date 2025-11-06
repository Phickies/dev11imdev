using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerControllers : MonoBehaviour
    {
        [Header("References")]
        public GameManager gameManager;
        private CharacterController controller;
        private PlayerManager player;

        [Header("Player Attribute Settings")]
        private float playerMass = 70f; // Player weight in kg
        private float airDrag = 0.1f;   // Air resistance

        [Header("Movement Settings")]
        public float walkSpeed = 5f;
        public float jumpHeight = 3.5f;
        public float airControl = 6f;

        [Header("Shooting Settings")]
        public GameObject bulletPrefab;
        public float bulletSpeed = 20f;
        public float offsetAim = 0.8f;
        public float offsetShoot = 1.0f;

        [Header("Slam Settings")]
        private float slamSpeed = -40f; // Slam downwards speed

        [Header("Dash Settings")]
        public float dashSpeed = 20f;
        public float dashDuration = 0.2f;

        private Vector3 baseVelocity;   // walk + gravity + jump
        private Vector3 dashVelocity;   // additive dash layer
        private float dashTimeRemaining;

        [Header("Input")]
        private float horizontalInput;
        private float verticalInput;
        private bool jumpInput;

        [Header("Attack Settings")]
        public GameObject katanaPrefab;
        public float katanaDamage = 30f;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            player = GetComponent<PlayerManager>();
        }

        private void Update()
        {
            if (player != null && player.IsDead())
                return;

            HandleInput();
            UpdateDash();
            UpdateBaseMovement();
            //keep adding updates here for other motion layers

            // Combine all motion layers
            Vector3 totalVelocity = baseVelocity + dashVelocity;

            controller.Move(totalVelocity * Time.deltaTime);
        }

        private void HandleInput()
        {
            // Cancel opposite keys (A+D / W+S)
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
                horizontalInput = 0f;
            else
                horizontalInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
                verticalInput = 0f;
            else
                verticalInput = Input.GetAxisRaw("Vertical");

            jumpInput = Input.GetButtonDown("Jump");
        }

        private void UpdateBaseMovement()
        {
            float currentSpeed = walkSpeed;
            Vector3 inputDir = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            inputDir = transform.TransformDirection(inputDir);

            // Grounded movement
            if (controller.isGrounded)
            {
                baseVelocity.x = inputDir.x * currentSpeed;
                baseVelocity.z = inputDir.z * currentSpeed;

                if (jumpInput)
                    baseVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gameManager.GetGravity());
                else if (baseVelocity.y < 0f)
                    baseVelocity.y = -2f; // Keep grounded
            }
            else
            {
                // Air control interpolation
                Vector3 targetVelocity = inputDir * currentSpeed;
                baseVelocity.x = Mathf.Lerp(baseVelocity.x, targetVelocity.x, airControl * Time.deltaTime);
                baseVelocity.z = Mathf.Lerp(baseVelocity.z, targetVelocity.z, airControl * Time.deltaTime);
            }

            ApplyGravity();
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded && baseVelocity.y < 0)
            {
                baseVelocity.y = -2f;
            }
            else
            {
                baseVelocity.y += gameManager.GetGravity() * Time.deltaTime;

                // Apply drag proportional to player mass
                float dragEffect = airDrag / (playerMass / 70f);
                baseVelocity.y *= 1f - dragEffect * Time.deltaTime;
            }
        }

        public void ForceJump()
        {
            baseVelocity.y = Mathf.Sqrt(jumpHeight* 1.5f * -2f * gameManager.GetGravity());
        }

        public void Dash()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;

            Vector3 dashDir = mainCamera.transform.forward;
            dashDir.Normalize();

            dashVelocity = dashDir * dashSpeed;
            dashTimeRemaining = dashDuration;
        }

        private void UpdateDash()
        {
            if (dashTimeRemaining <= 0f)
                return;

            dashTimeRemaining -= Time.deltaTime;

            // Smoothly decay dash over duration (frame-rate independent)
            float t = Mathf.Clamp01(1f - dashTimeRemaining / dashDuration);
            dashVelocity = Vector3.Lerp(dashVelocity, Vector3.zero, t);

            if (dashTimeRemaining <= 0f)
                dashVelocity = Vector3.zero;
        }

        public void Slam()
        {
            baseVelocity.x = 0f;
            baseVelocity.z = 0f;
            baseVelocity.y = slamSpeed;
        }
        public void Shoot()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;

            Vector3 shootDir = mainCamera.transform.forward;

            // Spawn bullet slightly offset to the right & forward
            Vector3 spawnPos = mainCamera.transform.position +
                               mainCamera.transform.right * offsetAim +
                               shootDir * offsetShoot;

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(shootDir));

            if (bullet.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rb.linearVelocity = shootDir * bulletSpeed;

                if (TryGetComponent<Collider>(out var playerCol) &&
                    bullet.TryGetComponent<Collider>(out var bulletCol))
                {
                    Physics.IgnoreCollision(playerCol, bulletCol);
                }
            }
        }

        public void UseKatana()
        {
            GameObject slash = Instantiate(katanaPrefab, transform.position + transform.forward * 1.2f, transform.rotation);
            if (slash.TryGetComponent(out KatanaObject katana))
            {
                katana.Initialize(transform);
            }
        }
        public void WarpTo(Vector3 newPosition)
        {
            if (controller == null)
                controller = GetComponent<CharacterController>();

            controller.enabled = false;
            transform.position = newPosition;
            controller.enabled = true;

            baseVelocity = Vector3.zero; // reset gravity 
        }

    }
}
