using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PileOfCards : MonoBehaviour, ISelectionable
{
    [SerializeField] List<Card> currentPileOfCards;
    [SerializeField] int currentShowingCard = -1;
    [SerializeField] private SlotCardAttacher hidedPileSlot;
    [SerializeField] private SlotCardAttacher showedPileSlot;

    public void SetPileOfCards(List<Card> newPileOfCards)
    {
        if (currentPileOfCards.Count > 0)
        {
            foreach (var card in currentPileOfCards)
            {
                this.DestroyGameObjectInAnyContext(card.gameObject);
            }
        }
        currentPileOfCards = newPileOfCards;
        hidedPileSlot.ClearNulls();
        for (var index = 0; index < newPileOfCards.Count; index++)
        {
            var card = newPileOfCards[index];
            card.gameObject.name += index;
            hidedPileSlot.AttachCard(card);
        }
        hidedPileSlot.ApplyChanges();
        showedPileSlot.ApplyChanges();
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
                HidePreviousCard(currentShowingCard);
            }
            currentShowingCard ++;
            ShowCard(currentShowingCard);
        }
    }

    private void HidePreviousCard(int previousToIndex)
    {
        Card previousCard = currentPileOfCards[previousToIndex];
        previousCard.DecreasePriority();
        previousCard.ToggleSelectable(false);
    }

    private void ShowCard(int cardNumber)
    {
        currentShowingCard = cardNumber;
        Debug.Log("Showing card" + currentShowingCard);
        var currentCard = currentPileOfCards[currentShowingCard];
        currentCard.IncreasePriority(1);
        currentCard.ToggleSelectable(true);
        currentCard.Show();
        hidedPileSlot.DeAttachCard(currentCard);
        showedPileSlot.AttachCard(currentCard);
    }

    public void ClosePileOfCards()
    {
        for (var index = 0; index < currentPileOfCards.Count; index++)
        {
            var card = currentPileOfCards[index];
            showedPileSlot.DeAttachCard(card,false);
            hidedPileSlot.AttachCard(card,false);
            card.RestorePriority();
            card.Hide();
        
            card.transform.parent = transform;
            card.transform.localPosition = Vector3.zero;
        }
        currentShowingCard = -1;

    }

    public void OnCardAttached(Card attachedCard)
    {
        if (!currentPileOfCards.Contains(attachedCard))
        {
            int placeToInsert = currentShowingCard == -1 ? 0 : currentShowingCard;
            currentPileOfCards.Insert(placeToInsert, attachedCard);
            if (placeToInsert - 1 != -1)
            {
                HidePreviousCard(currentShowingCard);
            }
            ShowCard(placeToInsert);
        }
    }

    public void NotifyCardIsPlaced(Card card)
    {
        var currentSlotAttacher = card.GetComponentInParent<SlotCardAttacher>();
        if (currentSlotAttacher != showedPileSlot)
        {
            ShowPreviousCard();
            currentPileOfCards.Remove(card);
            currentShowingCard = Mathf.Max(currentShowingCard - 1, -1);
        }
        else if (currentSlotAttacher == hidedPileSlot)
        {
            //    currentPileOfCards.Insert(currentShowingCard+1,card);
            //   currentShowingCard = Mathf.Min(currentShowingCard +1, currentPileOfCards.Count);
        }
    }

    public void ShowPreviousCard()
    {
        if (currentShowingCard > 0)
        {
            ShowCard(currentShowingCard - 1);
        }
    }

    public void OnSelected(GameObject selectedGameObject)
    {
        ShowNextCard();
    }
}