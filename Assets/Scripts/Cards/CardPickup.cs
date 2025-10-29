using UnityEngine;

public class CardPickup : MonoBehaviour
{
    public Card cardToGive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CardManager cardManager = other.GetComponent<CardManager>();

        if (cardManager != null)
        {
            Debug.Log("Player picked up a card: " + cardToGive.name);
            cardManager.AddCard(cardToGive);
        }
        Destroy(gameObject);
    }
}
