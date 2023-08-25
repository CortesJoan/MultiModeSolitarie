using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "new CardIsCardType", menuName = "CardIsCardType", order = 100)]
    public class CardIsCardType :SlotConditions
    {
        [SerializeField] CardType cardType;
            
        public override bool IsConditionMet()
        {               
            return  cardToCheck.GetCardType() == cardType;
        }
    }
}