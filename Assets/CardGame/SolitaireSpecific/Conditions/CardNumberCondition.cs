using System;
using UnityEngine;

[AddTypeMenu("SlotCondition/CardNumber condition")]
[Serializable]
public class CardNumberCondition : ISlotCondition
{
    [SerializeField] private CardNumber neededCardNumber;
    [SerializeField] private NumberCondition numberCondition;

    bool CheckNumberCondition(Card cardToCheck)
    {
        CardNumber currentCardNumber = cardToCheck.GetCardNumber();
        var isConditionMet = numberCondition switch
        {
            NumberCondition.Increase => currentCardNumber == neededCardNumber,
            NumberCondition.Decrease => currentCardNumber == neededCardNumber || neededCardNumber == CardNumber.Any,
            NumberCondition.Equals => currentCardNumber == neededCardNumber,
            NumberCondition.NotEquals => currentCardNumber != neededCardNumber,
            _ => throw new ArgumentOutOfRangeException()
        };
        return isConditionMet;
    }

    enum NumberCondition
    {
        Increase,
        Decrease,
        Equals,
        NotEquals
    }


    public bool IsConditionMet(Card cardToValue)
    {
        return CheckNumberCondition(cardToValue);
    }

//todo OnConditionIsMet and UpdateDefaultCondition is too similar
    public void OnConditionIsMet(Card cardToCheck)
    {
        CardNumber currentCardNumber = cardToCheck.GetCardNumber();
        switch (numberCondition)
        {
            case NumberCondition.Increase:

                if (neededCardNumber == CardNumber.Any)
                {
                    neededCardNumber = currentCardNumber;
                }

                neededCardNumber = currentCardNumber + 1;

                break;
            case NumberCondition.Decrease:

                neededCardNumber = currentCardNumber - 1;

                break;
        }
    }

    public void UpdateDefaultCondition(Card cardToValue)
    {
        CardNumber cardNumber = cardToValue.GetCardNumber();

        switch (numberCondition)
        {
            case NumberCondition.Increase:
                neededCardNumber = cardNumber + 1;
                break;
            case NumberCondition.Decrease:
                neededCardNumber = cardNumber - 1;
                break;
            case NumberCondition.Equals:
                neededCardNumber = cardNumber;
                break;
            case NumberCondition.NotEquals:
//                neededCardNumber = cardNumber;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public ISlotCondition Clone()
    {
        return new CardNumberCondition()
        {
            neededCardNumber = this.neededCardNumber,
            numberCondition = this.numberCondition
        };
    }
}