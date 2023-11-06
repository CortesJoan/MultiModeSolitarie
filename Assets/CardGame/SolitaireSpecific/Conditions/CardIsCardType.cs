using System;
using UnityEngine;

[AddTypeMenu("SlotCondition/CardType condition")]
[Serializable]
public class CardIsCardType : ISlotCondition
{
    [SerializeField] private CardType cardType;



    public bool IsConditionMet(Card cardToValue)
    {
        return cardToValue.GetCardType() == cardType;
    }

    public void OnConditionIsMet(Card cardToValue)
    {
        
    }

    public void UpdateDefaultCondition(Card cardToValue)
    {
        cardType = cardToValue.GetCardType();
    }
    public ISlotCondition Clone()
    {
        return new CardIsCardType()
        {
            cardType = this.cardType
        };
    }
}