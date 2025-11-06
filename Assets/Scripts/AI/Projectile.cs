using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 22f;
    public float life = 4f;
    public int damage = 10;

    Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }
    void Start() { Destroy(gameObject, life); }

    // call this right after Instantiate
    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit somehting");
        PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();
        if (player != null)
        {

            //poop my pants
            player.TakeDamage(10); 
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.GetComponent<EnemyController>() != null){
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