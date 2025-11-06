using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerBody;

        [Header("Camera Settings")]
        [SerializeField] private Vector3 offset = new (0f, 1.7f, 0f);

        [Header("Mouse Settings")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float minVerticalAngle = -90f;
        [SerializeField] private float maxVerticalAngle = 90f;
        [SerializeField] private bool smoothCamera = true;
        [SerializeField] private float smoothSpeed = 10f;

        private float xRotation = 0f;
        private float targetXRotation = 0f;
        private float targetYRotation = 0f;
        public GameObject pauseMenuManager;

        private void Start()
        {
            // Lock and hide cursor for FPS experience
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Offset the camera for player view
            gameObject.transform.position = playerBody.transform.position + offset;
            
            // Find player body if not assigned
            if (playerBody == null && transform.parent != null)
            {
                playerBody = transform.parent;
            }
        }

#pragma warning disable S2325
        private void Update()
        {
            // Check if already included pause menu manager
            if (pauseMenuManager != null)
            {
                PauseMenu manager = pauseMenuManager.GetComponent<PauseMenu>();
                if (manager.isPaused)
                {
                    return;
                }
            }
            // Handle cursor lock/unlock in Update (input detection)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // Click to lock cursor again
            if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        #pragma warning restore S2325

        private void LateUpdate()
        {
            // Handle camera rotation in LateUpdate to ensure smooth movement after player updates
            HandleFirstPersonCamera();
        }

        // Handle first-person camera logic
        private void HandleFirstPersonCamera()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            // Get mouse input (NO Time.deltaTime - Input.GetAxis is already frame-rate independent!)
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Short way to handle if else for smooth vs direct camera movement
            (smoothCamera ? (System.Action<float, float>)HandleSmoothCameraView : HandleDirectCameraView)(mouseX, mouseY);
        }

        // Smooth camera rotation for buttery-smooth feel
        private void HandleSmoothCameraView(float mouseX, float mouseY)
        {
            targetXRotation -= mouseY;
            targetXRotation = Mathf.Clamp(targetXRotation, minVerticalAngle, maxVerticalAngle);
            targetYRotation += mouseX;

            // Smoothly interpolate to target rotation
            xRotation = Mathf.Lerp(xRotation, targetXRotation, smoothSpeed * Time.deltaTime);
            float currentYRotation = Mathf.LerpAngle(playerBody != null ? playerBody.eulerAngles.y : 0f, targetYRotation, smoothSpeed * Time.deltaTime);

            // Apply vertical rotation to camera
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Apply horizontal rotation to player body
            if (playerBody != null)
            {
                playerBody.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
            }
        }

        // Direct, instant camera rotation (no smoothing)        
        private void HandleDirectCameraView(float mouseX, float mouseY)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

            // Apply vertical rotation to camera
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Apply horizontal rotation to player body
            if (playerBody != null)
            {
                playerBody.Rotate(Vector3.up * mouseX);
            }
        }
    }
}
