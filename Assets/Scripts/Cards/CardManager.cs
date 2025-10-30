using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<Card> availableCards = new List<Card>();
    public PlayerController controller;
    public CardUIManager cardUIManager;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCard(Card card)
    {
        availableCards.Add(card);
        if(card.effect != null)
        {
            card.effect.ApplyEffect(controller);
            cardUIManager.AddCardToUI(card);
        }
        Debug.Log("Picked up " + card.name);
    }
    void RemoveCard(Card card)
    {

    }
    void ActivateCard()
    {
        
    }
}
