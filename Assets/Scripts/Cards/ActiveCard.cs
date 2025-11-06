using UnityEngine;

public class ActiveCard
{
    public Card card;
    public float timeRemaining;

    public ActiveCard(Card card)
    {
        this.card = card;
        timeRemaining = card.duration;
    }
}
