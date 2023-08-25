using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotCardAttacher : MonoBehaviour
{
  [SerializeField]  List<Card> attachedCards = new List<Card>();
    [SerializeField] private List<SlotConditions> slotConditions = new List<SlotConditions>();

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AttachCard(Card card)
    {
        if (slotConditions.TrueForAll(a=>a.IsConditionMet()))
        {
            attachedCards.Add(card);

        }
    }

    public void DeAttachCard(Card card)
    {
        attachedCards.Remove(card);
    }

    public List<Card> GetAttachedCards()
    {
        return attachedCards;
    }
}

public class SlotConditions : Condition 
{
    protected Card cardToCheck;
    public override bool IsConditionMet()
    {
        return true;
    }

    public void SetCardCondition(Card card)
    {
        cardToCheck = card;
    }
}