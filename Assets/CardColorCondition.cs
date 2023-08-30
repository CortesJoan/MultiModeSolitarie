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

    public bool IsConditionMet(Card cardToValue)
    {
        bool isConditionMet = false;

        isConditionMet = cardToValue.GetCardColor() == neededColor ||neededColor == CardColor.Any ;
        if ((isConditionMet && (changeColorWhenAttachingCard) || neededColor == CardColor.Any))
        {
            UpdateDefaultCondition(cardToValue);
        }
        return isConditionMet;
    }

    public void UpdateDefaultCondition(Card cardToValue)
    {
        neededColor = cardToValue.GetCardColor() == CardColor.Black ? CardColor.Red : CardColor.Black;
    }
}