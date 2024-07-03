using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    [SerializeField] CardType cardType;
    [SerializeField] CardNumber cardNumber;
//todo improve code related to card color
    [SerializeField] CardColor cardColor;
    [SerializeField] private bool cardIsSelectable = true;
    [SerializeField] private int currentPriority = 0;
    private const int maxPriority = 10;
    [SerializeField] private bool isShowed;
    [SerializeField] Collider2D cardTrigger;

    public UnityEvent<CardType> onCardTypeChanged;
    public UnityEvent onCardHided;
    public UnityEvent onCardShowed;
    public UnityEvent onCardIsSelected;
    public UnityEvent onCardIsUnSelected;
    public UnityEvent<Collider2D> onCardTriggeredCollider;
    public UnityEvent<CardNumber> onCardNumberChanged;
    public UnityEvent<int> onPriorityIncreased;
    public UnityEvent<int> onPriorityDecreased;

  
    public void Show()
    {
        isShowed = true;
        onCardShowed?.Invoke();
    }

    public void Hide()
    {
        isShowed = false;
        onCardHided?.Invoke();
    }

    public bool IsShown()
    {
        return isShowed;
    }

    public void DecreasePriority(int amount=1)
    {
        currentPriority-=amount;
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
        currentPriority = Mathf.Min(maxPriority, increaseValue);
        onPriorityIncreased?.Invoke(currentPriority);
    }

    public void RestorePriority()
    {
        if (currentPriority > 0)
        {
            currentPriority = 1;
            onPriorityDecreased?.Invoke(currentPriority);
        }
        else
        {
            currentPriority = 1;
            onPriorityIncreased?.Invoke(currentPriority);
        }
    }

    public void SetPriority(int value)
    {
        if (currentPriority > value)
        {
            currentPriority = value;
            onPriorityDecreased?.Invoke(currentPriority);
        }
        else
        {
            currentPriority = value;
            onPriorityIncreased?.Invoke(currentPriority);
        }
    }
    public CardType GetCardType()
    {
        return cardType;
    }

    public void SetCardType(CardType newCardType)
    {
        cardType = newCardType;
        cardColor = newCardType == CardType.Pikes || newCardType == CardType.Clover ? CardColor.Black : CardColor.Red;
        onCardTypeChanged?.Invoke(newCardType);
    }

    public CardNumber GetCardNumber()
    {
        return cardNumber;
    }  public CardColor GetCardColor()
    {
        return cardColor;
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

    public override string ToString()
    {
        return cardType.ToString() + " " + cardNumber.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        onCardTriggeredCollider?.Invoke(other);
    }
    public void ToggleCardTrigger(bool state) {
     cardTrigger.enabled = state;
    
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
    King,
    Any
}

public enum CardType
{
    Hearts,
    Tiles,
    Clover,
    Pikes
}

public enum CardColor
{
    Red,
    Black,
    Any
}