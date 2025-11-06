using UnityEngine;

public class KatanaObject : MonoBehaviour
{
    private Transform player;
    private float radius = 2f;
    private float swingAngle = 90f; // degrees to each side
    private float duration = 0.4f;
    private float timer;

    private Vector3 centerOffset;
    private Vector3 startDirection;
    private Vector3 endDirection;

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        timer = duration;

        // Set up swing arc
        centerOffset = player.position + Vector3.up * 1.2f;
        startDirection = Quaternion.Euler(0, -swingAngle, 0) * player.forward;
        endDirection = Quaternion.Euler(0, swingAngle, 0) * player.forward;

        // Start position at left side of swing
        transform.position = centerOffset + startDirection * radius;
        transform.rotation = Quaternion.LookRotation(player.forward, Vector3.up);
    }

    private void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // Calculate interpolation 0 → 1
        float t = 1f - (timer / duration);
        // Smooth motion (ease in/out)
        t = Mathf.SmoothStep(0, 1, t);

        // Interpolate swing direction
        Vector3 currentDir = Vector3.Slerp(startDirection, endDirection, t);
        transform.position = centerOffset + currentDir * radius;
        transform.rotation = Quaternion.LookRotation(currentDir, Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            
            Debug.Log("beat his ass");
            enemy.GetHit(30);
        }
    }
}
