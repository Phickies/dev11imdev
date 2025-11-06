using UnityEngine;

public class CardPickup : MonoBehaviour
{
    public Card cardToGive;
    public AudioClip pickupSound; 
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

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

            if (pickupSound != null){
                  AudioSource.PlayClipAtPoint(pickupSound, other.transform.position);
        }
    }
        Destroy(gameObject);
    }
}
