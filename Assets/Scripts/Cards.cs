using UnityEngine;

// Card data structure to hold card information
[System.Serializable]
public class CardData
{
    public string cardName;
    public EffectType effectType;
    public float value;
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

    // Get a random card from available cards
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

    // Generate a random card with random properties
    public CardData GenerateRandomCard()
    {
        EffectType[] effectTypes = { EffectType.Heal, EffectType.Dash, EffectType.Slow, EffectType.JumpBoost };
        EffectType randomEffect = effectTypes[Random.Range(0, effectTypes.Length)];

        CardData newCard = new();

        switch (randomEffect)
        {
            case EffectType.Heal:
                newCard.cardName = "Heal Card";
                newCard.effectType = EffectType.Heal;
                newCard.value = Random.Range(10f, 50f);
                break;

            case EffectType.Dash:
                newCard.cardName = "Dash Card";
                newCard.effectType = EffectType.Dash;
                newCard.value = Random.Range(5f, 15f);
                break;

            case EffectType.Slow:
                newCard.cardName = "Slow Card";
                newCard.effectType = EffectType.Slow;
                newCard.value = Random.Range(2f, 5f);
                break;

            case EffectType.JumpBoost:
                newCard.cardName = "Jump Boost Card";
                newCard.effectType = EffectType.JumpBoost;
                newCard.value = Random.Range(2f, 8f);
                break;
        }

        return newCard;
    }

    // Select a card slot and generate a random card if empty
    public void SelectCardSlot(int slotIndex, PlayerManager player)
    {
        if (player == null) return;

        CardData cardInSlot = player.GetCard(slotIndex);

        if (cardInSlot == null)
        {
            cardInSlot = GenerateRandomCard();
            player.SetCard(slotIndex, cardInSlot);
        }

        player.SetSelectedIndex(slotIndex);

        // Log card selection
        if (cardInSlot != null)
        {
            Debug.Log($"Selected card {slotIndex + 1}: {cardInSlot.cardName} - {cardInSlot.effectType} (Value: {cardInSlot.value})");
        }
        else
        {
            Debug.Log($"Selected slot {slotIndex + 1}: Empty");
        }
    }

    // Get the currently selected card from player
    public CardData GetSelectedCard(PlayerManager player)
    {
        if (player == null) return null;

        int selectedIndex = player.GetSelectedIndex();
        if (selectedIndex >= 0)
        {
            return player.GetCard(selectedIndex);
        }

        return null;
    }

    // Clear the selected card after use (handles infinite effects)
    public void ClearSelectedCard(PlayerManager player)
    {
        if (player == null) return;

        int selectedIndex = player.GetSelectedIndex();
        if (selectedIndex < 0) return;

        CardData selectedCard = player.GetCard(selectedIndex);

        // Check if we should keep the card based on infinite effect settings
        if (player.ShouldKeepCard(selectedCard))
        {
            Debug.Log($"Infinite {selectedCard.effectType}: Card not consumed");
        }
        else
        {
            // Clear the card from the hand
            player.ClearCard(selectedIndex);
            player.SetSelectedIndex(-1);

            if (selectedCard != null)
            {
                Debug.Log($"Card consumed: {selectedCard.cardName}");
            }
        }
    }
}
