using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PileOfCards : MonoBehaviour
{
    [SerializeField] List<Card> currentPileOfCards;
    int currentShowingCard = -1;
    [SerializeField] private SlotCardAttacher showedPileSlot;

    public void SetPileOfCards(List<Card> newPileOfCards)
    {
        currentPileOfCards = newPileOfCards;
    }

    public List<Card> GetPileOfCards()
    {
        return currentPileOfCards;
    }

    public void ShowNextCard()
    {
        if ((currentShowingCard + 1) >= currentPileOfCards.Count)
        {
            ClosePileOfCards();
        }
        else
        {
            if (currentShowingCard != -1)
            {
                Card previousCard = currentPileOfCards[currentShowingCard];
                previousCard.DecreasePriority();
                showedPileSlot.DeAttachCard(previousCard);

            }
            currentShowingCard++;
            Debug.Log("Showing card" + currentShowingCard);
            var currentCard = currentPileOfCards[currentShowingCard];


            currentCard.Show();
            currentCard.transform.parent = showedPileSlot.transform;
 
            showedPileSlot.AttachCard(currentCard);
            currentCard.transform.localPosition = Vector3.zero;
        }
    }

    public void ClosePileOfCards()
    {
        currentShowingCard = -1;
        foreach (var card in currentPileOfCards)
        {
            card.transform.parent = transform;
            card.transform.localPosition = Vector3.zero;
            card.Hide();
            showedPileSlot.DeAttachCard(card);
        }
    }
}