using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDataBank : MonoBehaviour
{

    [SerializeField] List<CardTypeNumber> cardSprites;
    public static VisualDataBank instance;
    /*{

        get
        {
            if (instance == null)
                instance = FindObjectOfType<VisualDataBank>();
            return instance;
        }
        set 
        {
            instance = value;
        }
    }*/
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

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
