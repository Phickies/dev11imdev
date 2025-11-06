using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 22f;
    public float life  = 4f;
    public int damage  = 10;
    public LayerMask hitMask; // e.g. Player layer

    Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }
    void Start() { Destroy(gameObject, life); }

    // call this right after Instantiate
    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        // only interact with layers we care about
        if ((hitMask.value & (1 << other.gameObject.layer)) == 0) return;

        // // example: apply damage if the thing has health
        // var hp = other.GetComponent<PlayerHealth>();
        // if (hp != null) hp.TakeDamage(damage);

        Destroy(gameObject);
    }
}
