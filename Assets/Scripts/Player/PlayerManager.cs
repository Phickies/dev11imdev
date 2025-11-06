using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Effect types that can be applied to the player
public enum EffectType
{
    None,
    Heal,
    Dash,
    Slow,
    JumpBoost
}

[RequireComponent(typeof(CharacterController))]

public class PlayerManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Fall Damage Settings")]
    [SerializeField] private float fallDamageThreshold = 5f; // Minimum fall height before taking damage
    [SerializeField] private float fallDamageMultiplier = 5f; // Damage per meter fallen above threshold
    private CharacterController characterController;
    private EffectType currentEffect;

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = false;
    [SerializeField] public bool invincible = false;

    [Header("Zone Settings")]
    [SerializeField] private float startZoneVerticalOffset = 25f;
    [SerializeField] private Vector3 bossDefeatedTeleportPosition = new Vector3(0f, 45f, 0f);

    // Fall tracking variables
    private bool isFalling = false;
    public bool isDead = false;
    private float highestPoint;

    public Image damageImage;
    public float damageDuration;
    public GameObject deathUI;
    public GameManager gameManager;

    private bool damaged;
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip jumpSound;

    void Start()
    {
        currentHealth = maxHealth;

        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }
            if (gameManager == null)
            {
                Debug.LogError("PlayerManager could not find a GameManager instance in the scene.");
            }
        }

        highestPoint = transform.position.y;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        CheckFallDamage();
        if (!isDead && !damaged)
        {
            deathUI.SetActive(false);
        }
    }

    // Check for fall damage
    private void CheckFallDamage()
    {
        if (characterController == null) return;

        float currentY = transform.position.y;

        if (!characterController.isGrounded)
        {
            isFalling = true;

            // Track the highest point during the fall
            if (currentY > highestPoint)
            {
                highestPoint = currentY;
            }
        }
        else if (isFalling)
        {
            // Calculate fall distance
            float fallDistance = highestPoint - currentY;

            // Apply damage if fall distance exceeds threshold
            if (fallDistance > fallDamageThreshold)
            {
                float damageAmount = (fallDistance - fallDamageThreshold) * fallDamageMultiplier;

                // Only apply damage if it's greater or equal to 10
                if (damageAmount >= 10)
                {
                    TakeDamage((int)damageAmount);
                    // Debug.Log removed for performance - was causing lag during gameplay
                }
            }

            // Reset fall tracking
            isFalling = false;
            highestPoint = currentY;
        }
        else
        {
            // Update highest point to current position when grounded
            highestPoint = currentY;
        }
    }

    // Apply damage to the player
    public void TakeDamage(int damage)
    {
        // If invincible, ignore all damage
        if (invincible)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"Invincible mode: Blocked {damage} damage");
            }
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        FlashDamage();
        PlayDamageSound();

        if (enableDebugLogs)
        {
            Debug.Log($"Took {damage} damage. Current health: {currentHealth}/{maxHealth}");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Heal the player
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private IEnumerator FlashCoroutine()
    {
        damaged = true;
        damageImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(damageDuration);
        damageImage.gameObject.SetActive(false);
        damaged = false;
    }

    public void FlashDamage()
    {
        if (damageImage != null)
            StartCoroutine(FlashCoroutine());
    }

    // Handle player death
    private void Die()
    {
        Debug.Log("Player has died!");
        isDead = true;
        deathUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    // Get current health value
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Get current effect of the player
    public EffectType GetCurrentEffect()
    {
        return currentEffect;
    }

    // Set current effect of the player
    public void SetCurrentEffect(EffectType effect)
    {
        currentEffect = effect;
    }

    // Get current state of dead
    public bool IsDead()
    {
        return isDead;
    }


    // Check if invincible is enabled
    public bool IsInvincible()
    {
        return invincible;
    }

    public void warpTo(Vector3 position)
    {
        PlayerControllers controller = GetComponent<PlayerControllers>();
        if (controller != null)
        {
            controller.WarpTo(position); // delegate to movement script
        }
        else
        {
            // fallback if no contrller attached
            transform.position = position;
        }

        // Reset fall-related state so the player doesn't take damage on load
        isFalling = false;
        highestPoint = position.y;

    }

    public void PlayDamageSound()
    {
        if (damageSound != null && audioSource != null)
            audioSource.PlayOneShot(damageSound);
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
            audioSource.PlayOneShot(jumpSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || isDead)
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

            case "Final round":
                gameManager.StartBossRave();
                break;
        }
    }

    private void HandleDeathZone()
    {
        if (invincible)
        {
            return;
        }

        int lethalDamage = Mathf.Max(1, currentHealth);
        TakeDamage(lethalDamage);
    }

    private void HandleStartZone()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.y += startZoneVerticalOffset;
        warpTo(targetPosition);
    }

    private void HandleFinishZone()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Player has reached the finish zone!");
    }

    private void HandleBossDefeatedZone()
    {
        warpTo(bossDefeatedTeleportPosition);
    }

    #region save and load

    public void Save(ref PlayerData data)
    {
        data.Position = transform.position;
        data.currentEff = GetCurrentEffect();
        data.currentHeath = currentHealth;

        Debug.Log("saved position: " + data.Position);
    }

    public void Load(PlayerData data)
    {
        transform.position = data.Position;
        warpTo(data.Position);
        currentEffect = data.currentEff;
        currentHealth = data.currentHeath;
        Debug.Log("new position: " + data.Position);
    }
    #endregion

}

[System.Serializable]
public struct PlayerData
{
    public Vector3 Position;
    public int currentHeath;
    public EffectType currentEff;
}


