using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentShowingCardUpdater : MonoBehaviour
{
    [SerializeField] private SlotCardAttacher slotCardAttacher;

    public void ShowNewLastCard()
    {
        if (transform.GetChild(transform.childCount - 1).TryGetComponent(out Card newLastCard))
        {
            newLastCard.ToggleSelectable(true);
            newLastCard.Show();
        }
    }
}