using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerCardManager : MonoBehaviour  
{
    List<Card> selectedCards = new List<Card>();
    public UnityEvent<Card>  onCardSelected = new UnityEvent<Card> ();
    public UnityEvent<Card> onCardUnSelected = new UnityEvent<Card> ();
    
    public void SelectCard(Card card)
    {
        selectedCards.Add(card);
        onCardSelected?.Invoke(card);
    }
    
    public void UnSelectCard(Card card)
    {
        selectedCards.Remove(card);
        onCardUnSelected?.Invoke(card);
    }

    public List<Card> GetSelectedCards()
    {
        return selectedCards;
    }

   
}
