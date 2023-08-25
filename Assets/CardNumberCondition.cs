using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new CardNumberCondition", menuName = "CardNumberCondition", order = 100)]

public class CardNumberCondition : SlotConditions
{
    [SerializeField] private CardNumber neededCardNumber;
    [SerializeField] private NumberCondition numberCondition;

    public override bool IsConditionMet()
    {
        return CheckNumberCondition();
    }

    bool CheckNumberCondition()
    {
        bool isConditionMet;
        switch (numberCondition)
        {
            case NumberCondition.Increase:

                isConditionMet = cardToCheck.GetCardNumber() > neededCardNumber;
                if (isConditionMet)
                {
                    neededCardNumber++;
                }
                break;
            case NumberCondition.Decrease:
                isConditionMet = cardToCheck.GetCardNumber() < neededCardNumber;

                if (isConditionMet)
                {
                    neededCardNumber--;
                }
                break;
            case NumberCondition.Equals:
                isConditionMet = cardToCheck.GetCardNumber() == neededCardNumber;
                break;
            case NumberCondition.NotEquals:
                isConditionMet = cardToCheck.GetCardNumber() != neededCardNumber;
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
}