using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCardManagerVisuals : MonoBehaviour
{
    [SerializeField] PlayerCardManager playerCardManager;
    private Vector2 pointerPosition = new Vector2();
    List<Card> currentSelectedCards =  new List<Card>();    
    private void OnEnable()
    {
        playerCardManager.onCardSelected.AddListener( OnCardSelected);
        playerCardManager.onCardUnSelected.AddListener(OnCardUnSelected);
    }

    private void Update()
    {
        if (currentSelectedCards.Count>0)
        {
            currentSelectedCards.First().transform.position = Camera.main.ScreenToWorldPoint(pointerPosition);
        }
    }

    public void OnCardSelected(Card selectedCard)
    {
        currentSelectedCards= playerCardManager.GetSelectedCards();
        selectedCard.transform.parent = currentSelectedCards.Last().transform;
        selectedCard.transform.localPosition = new Vector2(0, selectedCard.transform.localPosition.y + 1);
    }

    public void OnCardUnSelected(Card unSelectedCard)
    {        
        currentSelectedCards= playerCardManager.GetSelectedCards();

    }

    public void UpdatePointerPosition(InputAction.CallbackContext context)  
    {
        pointerPosition=  context.ReadValue<Vector2>();
    }
}