using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SlotCardAttacher : MonoBehaviour
{
    [SerializeField] List<Card> attachedCards = new List<Card>();
    [SerializeReference, SubclassSelector] private List<ISlotCondition> slotConditions = new List<ISlotCondition>();
    [SerializeField] public UnityEvent<Card> onAttachCard;
    [SerializeField] public UnityEvent<Card> onDeAttachCard;
    [SerializeReference, SubclassSelector]
    private List<ISlotCondition> slotConditionsWhenNoCardsAttached = new List<ISlotCondition>();
    [HideInInspector] [SerializeField] private float attachedCardsDistance = 0;
    [HideInInspector] [SerializeField] private List<Vector3> cachedAttachedPositions = new List<Vector3>();

    public void SetAttachedCardsDistance(float distance)
    {
        attachedCardsDistance = distance;
    }

    public bool TryToAttachCard(Card card)
    {
        bool succeedAttachingCard = false;
        if (CheckSlotConditions(card))
        {
            AttachCard(card);
            succeedAttachingCard = true;
        }
        return succeedAttachingCard;
    }

    public bool CheckSlotConditions(Card card)
    {
        return slotConditions.TrueForAll(a =>
        {
            if (a != null)
            {
                return a.IsConditionMet(card);
            }
            return true;
        });
    }

    public void AttachCard(Card card)
    {
        attachedCards.Add(card);
         Transform cardTransform = card.transform;
        cardTransform.SetParent(transform);
        int lastIndex = attachedCards.IndexOf(attachedCards.LastOrDefault());
        cardTransform.localPosition = Vector3.down * (attachedCardsDistance * lastIndex);
        cardTransform.localPosition =
            new Vector3(cardTransform.localPosition.x, cardTransform.localPosition.y, 0 - lastIndex - 1);
        onAttachCard?.Invoke(card);
        cachedAttachedPositions.Add(card.transform.position);
        Debug.Log($"Card attached " + card.ToString() + " to " + transform.name);
        int minimumPriority = 1;
        for (int i = 0; i < attachedCards.Count; i++)
        {
            attachedCards[i].SetPriority(minimumPriority+i);
        }
    }

    public void DeAttachCard(Card card)
    {
        card.DecreasePriority();

        attachedCards.Remove(card);
        onDeAttachCard?.Invoke(card);
        Debug.Log($"Card deattached " + card.ToString() + " to " + transform.name);
        if (attachedCards.Count == 0)
        {
            slotConditions = slotConditionsWhenNoCardsAttached;
        }
        else
        {
            foreach (var slotCondition in slotConditions)
            {
                slotCondition.UpdateDefaultCondition(attachedCards[^1]);
            }
        }
    }

    public List<Card> GetAttachedCards()
    {
        return attachedCards;
    }

    public List<Card> GetAttachedCardsFromStartingCard(Card startCard)
    {
        int startingCardIndex = attachedCards.IndexOf(startCard);
        return (startingCardIndex + 1) < attachedCards.Count
            ? attachedCards.GetRange(startingCardIndex, attachedCards.Count - startingCardIndex)
            : new List<Card>(1) { startCard };
    }
}