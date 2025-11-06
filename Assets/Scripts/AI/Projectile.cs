using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 22f;
    public float life = 4f;
    public int damage = 10;
    [Tooltip("Optional layer filter. Leave empty to hit everything.")]
    public LayerMask hitMask;

    Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }
    void Start() { Destroy(gameObject, life); }

    // call this right after Instantiate
    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private bool ShouldProcess(GameObject target)
    {
        // treat an empty mask the same as 'everything'
        if (hitMask.value == 0) return true;
        return (hitMask.value & (1 << target.layer)) != 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit somehting");
        if (!ShouldProcess(collision.gameObject))
        {
            Destroy(gameObject);
            return;
        }

        PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyController>() != null)
        {
            return;
        }

        if (!ShouldProcess(other.gameObject))
        {
            return;
        }

        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            Debug.Log("player took damage");
            player.TakeDamage(damage);
        }
        Destroy(gameObject);
    }



}
