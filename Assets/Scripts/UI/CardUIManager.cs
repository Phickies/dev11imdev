using TMPro;
using UnityEngine;


public class CardUIManager : MonoBehaviour
{
    public TextMeshProUGUI listOfCardsText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCardToUI(Card card)
    {
        listOfCardsText.text += "\n" + card.cardName;
    }
}
