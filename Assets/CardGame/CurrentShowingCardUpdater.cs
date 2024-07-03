using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentShowingCardUpdater : MonoBehaviour, ICardPlacedOnAnotherPlace
{
    [SerializeField] private SlotCardAttacher slotCardAttacher;

    public void NotifyCardIsPlacedOnAnotherPlace(Card card)
    {
        ShowNewLastCard();
    }

    public void ShowNewLastCard()
    {
        if (transform.GetChild(transform.childCount - 1).TryGetComponent(out Card newLastCard))
        {
            if (!newLastCard.IsShown())
            {
                newLastCard.ToggleSelectable(true);
                newLastCard.Show();   
            }
        }
    }
}