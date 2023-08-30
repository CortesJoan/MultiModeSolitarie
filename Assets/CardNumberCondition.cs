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
        bool isConditionMet;
        CardNumber cardNumber = cardToCheck.GetCardNumber();
        switch (numberCondition)
        {
            case NumberCondition.Increase:

                isConditionMet = cardNumber == neededCardNumber;
                if (neededCardNumber == CardNumber.Any)
                {
                    neededCardNumber = cardToCheck.GetCardNumber();
                }
                if (isConditionMet || cardToCheck.GetCardNumber() == (CardNumber.Any))
                {
                    neededCardNumber = cardNumber + 1;
                }
                break;
            case NumberCondition.Decrease:
                isConditionMet = cardToCheck.GetCardNumber() == neededCardNumber || neededCardNumber == CardNumber.Any;

                if (isConditionMet)
                {
                    neededCardNumber = cardToCheck.GetCardNumber() - 1;
                }
                break;
            case NumberCondition.Equals:

                isConditionMet = cardNumber == neededCardNumber;
                break;
            case NumberCondition.NotEquals:
                isConditionMet = cardNumber != neededCardNumber;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
}