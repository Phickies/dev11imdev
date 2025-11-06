using UnityEngine;

public class CardFloat : MonoBehaviour
{
    public float floatHeight = 0.2f;   // how high it bobs
    public float floatSpeed = 2f;      // how fast it bobs

    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        // Sin wave bob
        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY + 1f, transform.position.z);
    }
}
