using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddTypeMenu("SlotCondition/CardColor condition")]
[Serializable]
public class CardColorCondition : ISlotCondition
{
    [SerializeField] private CardColor neededColor;
    [SerializeField] private bool changeColorWhenAttachingCard;
    private bool isConditionMet;

  

    public bool IsConditionMet(Card cardToValue)
    {
        isConditionMet = false;

        isConditionMet = cardToValue.GetCardColor() == neededColor ||neededColor == CardColor.Any ;
        return isConditionMet;
    }

    public void OnConditionIsMet(Card cardToValue)
    {
        if ((isConditionMet && (changeColorWhenAttachingCard) || neededColor == CardColor.Any))
        {
            UpdateDefaultCondition(cardToValue);
            isConditionMet = false;
        }
    }

    public void UpdateDefaultCondition(Card cardToValue)
    {
        neededColor = cardToValue.GetCardColor() == CardColor.Black ? CardColor.Red : CardColor.Black;
    }
    public ISlotCondition Clone()
    {
        return new CardColorCondition()
        {
            neededColor = this.neededColor,
            changeColorWhenAttachingCard = this.changeColorWhenAttachingCard
        }; 
    } 
}