using UnityEngine;

public class CardShadow : MonoBehaviour
{
    public float shadowHeight = 1f;

    void LateUpdate()
    {
        // Raycast to find floor
        if (Physics.Raycast(transform.parent.position, Vector3.down, out RaycastHit hit, 5f))
        {
            transform.position = hit.point + Vector3.up * shadowHeight;
        }

        // Always stay flat
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
