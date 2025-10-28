using UnityEngine;

// Card data structure to hold card information
[System.Serializable]
public class CardData
{
    public string cardName;
    public EffectType effectType;
    public float value; // For Heal: health amount, For Dash: distance, For Slow: duration in seconds
}

public class Cards : MonoBehaviour
{
    // Example card definitions - you can create these in the Inspector or via code
    [Header("Available Cards")]
    [SerializeField] private CardData[] availableCards;

    // Get a card by index
    public CardData GetCard(int index)
    {
        if (availableCards != null && index >= 0 && index < availableCards.Length)
        {
            return availableCards[index];
        }
        return null;
    }

    // Get a random card
    public CardData GetRandomCard()
    {
        if (availableCards != null && availableCards.Length > 0)
        {
            int randomIndex = Random.Range(0, availableCards.Length);
            return availableCards[randomIndex];
        }
        return null;
    }

    // Get number of available cards
    public int GetCardCount()
    {
        return availableCards != null ? availableCards.Length : 0;
    }
}
