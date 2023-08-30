using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PileOfCards : MonoBehaviour
{
    [SerializeField] List<Card> currentPileOfCards;
    int currentShowingCard = -1;
    [SerializeField] private SlotCardAttacher hidedPileSlot;
    [SerializeField] private SlotCardAttacher showedPileSlot;
    private int totalNumberOfCards = 0;

    public void SetPileOfCards(List<Card> newPileOfCards)
    {
        currentPileOfCards = newPileOfCards;
        foreach (var card in newPileOfCards)
        {
            hidedPileSlot.TryToAttachCard(card);
        }
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
            }
            currentShowingCard++;
            ShowCard(currentShowingCard);
        }
    }

    private void ShowCard(int cardNumber)
    {
        currentShowingCard = cardNumber;
        Debug.Log("Showing card" + currentShowingCard);

        var currentCard = currentPileOfCards[currentShowingCard];
        currentCard.IncreasePriority(1);
        currentCard.Show();
        hidedPileSlot.DeAttachCard(currentCard);
        showedPileSlot.TryToAttachCard(currentCard);
        currentCard.transform.parent = showedPileSlot.transform;
        currentCard.transform.localPosition = Vector3.zero;
    }

    public void ClosePileOfCards()
    {
        currentShowingCard = -1;
        var list = showedPileSlot.GetAttachedCards();
        for (var index = list.Count - 1; index >= 0; index--)
        {
            var card = list[index];
            card.RestorePriority();
            card.Hide();
            showedPileSlot.DeAttachCard(card);
            hidedPileSlot.AttachCard(card); 
            card.transform.parent = transform;
            card.transform.localPosition = Vector3.zero;
        }
    }

    public void NotifyCardIsPlaced(Card card)
    {
        var currentSlotAttacher = card.GetComponentInParent<SlotCardAttacher>();
        if (currentSlotAttacher!= showedPileSlot && currentSlotAttacher!= hidedPileSlot)
        {
            currentPileOfCards.Remove(card);
            currentShowingCard = Mathf.Max(currentShowingCard - 1, 0);
        }
    }

    public void ShowPreviousCard()
    {
        if (currentShowingCard > 0)
        {
            ShowCard(currentShowingCard - 1);
        }
    }
}