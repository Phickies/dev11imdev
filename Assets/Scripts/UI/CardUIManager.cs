using TMPro;
using UnityEngine;


public class CardUIManager : MonoBehaviour
{
    public TextMeshProUGUI listOfCardsText;
    private Card currentlyHighlightedCard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HighlightCard(Card card)
    {
        currentlyHighlightedCard = card;
        RedrawCardList();
    }

    public void AddCardToUI(Card card)
    {
        // Just redraw list, card list is in CardManager anyway
        RedrawCardList();
    }

    public void RemoveCardFromUI(Card card)
    {
        RedrawCardList();
    }

    void RedrawCardList()
    {
        listOfCardsText.text = "";

        foreach (var card in FindFirstObjectByType<CardManager>().availableCards)
        {
            if (card == currentlyHighlightedCard)
                listOfCardsText.text += $"\n<color=#FFD600>-> {card.cardName}</color>";  // highlighted
            else
                listOfCardsText.text += $"\n- {card.cardName}";
        }
    }
}
