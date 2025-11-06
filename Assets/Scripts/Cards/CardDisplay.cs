using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public CardPickup pickupScript; // Assign your ScriptableObject here in the prefab
    private SpriteRenderer sr;

    void Start()
    {
        transform.localPosition += new Vector3(0, 1.0f, 0);
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = pickupScript.cardToGive.cardSprite;
    }
}
