using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerCardManager : MonoBehaviour
{
    List<Card> selectedCards = new List<Card>();
    public UnityEvent<Card> onCardSelected = new UnityEvent<Card>();
    public UnityEvent<Card> onCardUnSelected = new UnityEvent<Card>();

    public void SelectCard(Card card)
    {
        selectedCards.Add(card);
        onCardSelected?.Invoke(card);
        card.onCardIsSelected?.Invoke();
        
        Debug.Log("Card Selected");
    }

    public void UnSelectCard(Card card)
    {
        selectedCards.Remove(card);
        onCardUnSelected?.Invoke(card);
        card.onCardIsUnSelected?.Invoke();
        Debug.Log("Card Unselected");
    }


    public List<Card> GetSelectedCards()
    {
        return selectedCards;
    }
}