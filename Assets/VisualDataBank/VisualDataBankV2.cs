using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisualDataBank", menuName = "Create VisualDataBank", order = 0)]
public class VisualDataBankV2 : ScriptableObject
{
    [SerializeField] List<CardTypeNumber> cardSprites;

    public Sprite GetCardVisual(CardType cardType, CardNumber cardNumber)
    {
        return cardSprites[(int)cardType].spriteNumber[(int)cardNumber];
    }

    [System.Serializable]
    class CardTypeNumber
    {
        public List<Sprite> spriteNumber;
    }
}