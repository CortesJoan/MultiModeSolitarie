using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour 
{
    [SerializeField] CardType cardType;
    [SerializeField] CardNumber cardNumber;

    public UnityEvent<CardType> onCardTypeChanged;
    public UnityEvent onCardHided;
    public UnityEvent onCardShowed;
   public UnityEvent onCardSelected;
    public UnityEvent<CardNumber> onCardNumberChanged;
    [SerializeField] private bool cardIsSelectable=true;
    public UnityEvent<int> onPriorityIncreased;
    public UnityEvent<int> onPriorityDecreased;
   [SerializeField] public int currentPriority=0;
    private const int maxPriority=10;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Show()
    {
        onCardShowed?.Invoke();
    }

    public void Hide()
    {
        onCardHided?.Invoke();
    }

    public void DecreasePriority()
    {
        currentPriority--;
        onPriorityDecreased?.Invoke(currentPriority);
    }

    [ContextMenu(nameof(SetMaxPriority))]
    public void SetMaxPriority()
    {
        currentPriority = maxPriority;
        onPriorityIncreased?.Invoke(currentPriority);
    }

    public void IncreasePriority(int increaseValue)
    {
        currentPriority = Mathf.Min(maxPriority,increaseValue) ;
        onPriorityIncreased?.Invoke(currentPriority);
    }

    public CardType GetCardType()
    {
        return cardType;
    }

    public void SetCardType(CardType newCardType)
    {
        cardType = newCardType;
        onCardTypeChanged?.Invoke(newCardType);
    }

    public CardNumber GetCardNumber()
    {
        return cardNumber;
    }

    public void SetCardNumber(CardNumber newCardNumber)
    {
        cardNumber = newCardNumber;
        onCardNumberChanged?.Invoke(newCardNumber);
    }

  

    public void ToggleSelectable(bool isSelectable)
    {
        cardIsSelectable = isSelectable;
        
    }

    public bool CanBeSelected()
    {
        return cardIsSelectable;
    }
}

public enum CardNumber
{
    Joker,
    Ace,
    Number2,
    Number3,
    Number4,
    Number5,
    Number6,
    Number7,
    Number8,
    Number9,
    Number10,
    Jack,
    Queen,
    King
}

public enum CardType
{
    Hearts,
    Tiles,
    Clover,
    Pikes
}