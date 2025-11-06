using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    /// <summary>
    /// Handles interactions when the player enters trigger zones.
    /// Attach this to the player so it can react to tagged trigger volumes.
    /// </summary>
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerZoneTrigger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerManager playerManager;

        [Header("Zone Settings")]
        [SerializeField] private float startZoneVerticalOffset = 25f;
        [SerializeField] private Vector3 bossDefeatedTeleportPosition = new Vector3(0f, 45f, 0f);

        [Header("Events")]
        [SerializeField] private UnityEvent onPlayerWin;

        private void Reset()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        private void Awake()
        {
            if (playerManager == null)
            {
                playerManager = GetComponent<PlayerManager>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null || playerManager == null)
            {
                return;
            }

            switch (other.tag)
            {
                case "Dead zone":
                    HandleDeathZone();
                    break;

                case "Start zone":
                    HandleStartZone();
                    break;

                case "Finish zone":
                    HandleFinishZone();
                    break;

                case "Defeated boss":
                    HandleBossDefeatedZone();
                    break;
            }
        }

        private void HandleDeathZone()
        {
            int lethalDamage = playerManager.GetCurrentHealth();
            if (lethalDamage <= 0)
            {
                lethalDamage = playerManager.IsInvincible() ? 0 : 1;
            }

            if (lethalDamage > 0)
            {
                playerManager.TakeDamage(lethalDamage);
            }
        }

        private void HandleStartZone()
        {
            Vector3 targetPosition = transform.position;
            targetPosition.y += startZoneVerticalOffset;
            playerManager.warpTo(targetPosition);
        }

        private void HandleFinishZone()
        {
            onPlayerWin?.Invoke();

            // Provide sensible defaults if no event is assigned.
            if (onPlayerWin == null || onPlayerWin.GetPersistentEventCount() == 0)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Player has reached the finish zone!");
            }
        }

        private void HandleBossDefeatedZone()
        {
            playerManager.warpTo(bossDefeatedTeleportPosition);
        }
    }
}
