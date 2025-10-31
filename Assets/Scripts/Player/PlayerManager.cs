using UnityEngine;

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
    [SerializeField] public bool infiniteDash = false;

    // Fall tracking variables
    private bool isFalling = false;
    private bool isDead = false;
    private float highestPoint;

    void Start()
    {
        currentHealth = maxHealth;

        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        highestPoint = transform.position.y;

        // Give dash card if infinite dash is enabled
        if (infiniteDash)
        {
            //TODO: implement
        }
    }

    void Update()
    {
        CheckFallDamage();
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

    // Handle player death
    private void Die()
    {
        Debug.Log("Player has died!");
        isDead = true;
        // Add your death logic here (e.g., respawn, game over, etc.)
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

    // Check if infinite dash is enabled
    public bool HasInfiniteDash()
    {
        return infiniteDash;
    }

    // Check if invincible is enabled
    public bool IsInvincible()
    {
        return invincible;
    }

}
