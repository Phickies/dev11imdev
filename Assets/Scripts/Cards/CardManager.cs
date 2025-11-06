using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<Card> availableCards = new List<Card>();
    public List<ActiveCard> activeCards = new List<ActiveCard>(); // Cards that are currently activated
    public PlayerControllers controller;
    public CardUIManager cardUIManager;
    public float cardCountdown = 10f;
    private int selectedIndex;

    void Start()
    {
       selectedIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(availableCards.Count == 0)
        {
            return; //this gave me so many errros bruh
        }
        float scroll = Input.mouseScrollDelta.y;
        if(scroll > 0f)
        {
            selectedIndex--;
            if(selectedIndex < 0)
            {
                // wrap around
                selectedIndex = availableCards.Count - 1;
            }
            cardUIManager.HighlightCard(availableCards[selectedIndex]);
        }
        else if(scroll < 0f)
        {
            selectedIndex++;
            if(selectedIndex >= availableCards.Count)
            {
                //wrap around
                selectedIndex = 0;
            }
            cardUIManager.HighlightCard(availableCards[selectedIndex]);
        }


        //check for input to activate card
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(availableCards.Count > 0)
            {
                ActivateCard(availableCards[selectedIndex]);
            }
        }


        // Timer update
        for (int i = activeCards.Count - 1; i >= 0; i--)
        {
            activeCards[i].timeRemaining -= Time.deltaTime;

            if (activeCards[i].timeRemaining <= 0)
            {
                activeCards[i].card.effect.RemoveEffect(controller);
                activeCards.RemoveAt(i);
            }
        }
    }

    public void AddCard(Card card)
    {
        availableCards.Add(card);
        if(card.effect != null)
        {
            cardUIManager.AddCardToUI(card);
            if(availableCards.Count == 1)
            {
                selectedIndex = availableCards.Count - 1;
                cardUIManager.HighlightCard(availableCards[selectedIndex]);
            }

        }
    }
    void ActivateCard(Card card)
    {
        card.effect.ApplyEffect(controller);
        if (card.duration > 0) // timed card
        {
            activeCards.Add(new ActiveCard(card));
        }
        availableCards.RemoveAt(selectedIndex);
        cardUIManager.RemoveCardFromUI(card);
        if(availableCards.Count > 0)
        {
            selectedIndex = selectedIndex % availableCards.Count;
            cardUIManager.HighlightCard(availableCards[selectedIndex]);
        }

    }
    void RemoveCard(Card card)
    {
        // Maybe discard feature eventually
    }
    public void Save(ref CardData cardData)
    {
        cardData.Cards = new List<Card>();

        foreach (var card in availableCards)
        {
            cardData.Cards.Add(card);
        }
    }
    public void Load(CardData cardData)
    {
        availableCards.Clear();

        foreach (var card in cardData.Cards)
        {
            AddCard(card);
        }
    }
}

[System.Serializable]
public struct CardData
{
    public List<Card> Cards;
}
