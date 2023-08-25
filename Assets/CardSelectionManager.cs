using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardSelectionManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private Camera mainCamera;
    private List<Card> selectedCards;
    private SlotCardAttacher slotCardAttacherTarget;
    private SlotCardAttacher slotCardAttacherFrom;
    private Vector3 offset;
    private bool dragging;
    [SerializeField] private PlayerCardManager playerCardManager;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.onActionTriggered += HandleInput;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.onActionTriggered -= HandleInput;
        }
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        if (context.action.name == "MousePosition")
        {
            OnMousePositionChanged(context);
        }
        else if (context.action.name == "ArrowKeys")
        {
            OnArrowKeysChanged(context);
        }
        else if (context.action.name == "CardSelected")
        {
            //         OnCardSelected(context);
        }
    }

    private void OnMousePositionChanged(InputAction.CallbackContext context)
    {
        if (!dragging || selectedCards.Count == 0) return;

        Vector2 mousePosition = context.ReadValue<Vector2>();
        Vector3 worldMousePosition =
            mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y,
                -mainCamera.transform.position.z));
        var followCard = selectedCards.First().transform;

        followCard.position = new Vector3(worldMousePosition.x + offset.x, worldMousePosition.y + offset.y,
            followCard.position.z);
    }

    private void OnArrowKeysChanged(InputAction.CallbackContext context)
    {
        if (!dragging || selectedCards.Count == 0) return;

        Vector2 arrowKeys = context.ReadValue<Vector2>();
        // Implement logic for moving the selected card to the next available position using arrow keys
    }

    private bool isSelectingCards = false;

    public void OnCardSelected(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        Vector2 mousePosition= Mouse.current.position.ReadValue();

        RaycastHit2D[] hits = Physics2D.RaycastAll(mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);
        foreach (var hit in hits)
        {
             
            if (hit.collider != null)
            {
                //todo move to interface
                PileOfCards pileOfCards =  hit.collider.GetComponentInParent<PileOfCards>();
                if (pileOfCards != null)
                {
                    pileOfCards.ShowNextCard();
                    return;
                }
                Card card = hit.collider.GetComponentInParent<Card>();
                if (card==null||  !card.CanBeSelected())
                {
                    return;
                }
           
            
            
                SlotCardAttacher slotCardAttacherOfCurrentCard = card.GetComponentInParent<SlotCardAttacher>();
                Debug.Assert(slotCardAttacherOfCurrentCard != null, $"Invalid slotCardAttacherOfCurrentCard");
                slotCardAttacherFrom = slotCardAttacherOfCurrentCard;
                if (slotCardAttacherFrom != null)
                {
                    dragging = true;
                    selectedCards = slotCardAttacherFrom.GetAttachedCards();

                    playerCardManager.SelectCard(card);

                    Vector3 cardPosition = card.transform.position;
                    Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x,
                        mousePosition.y, -mainCamera.transform.position.z));
                    offset = cardPosition - new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, cardPosition.z);
                    isSelectingCards = true;
                }
            }     
        }
  
    }

    public void OnCardUnSelected(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (isSelectingCards)
        {
            dragging = false;

            if (slotCardAttacherTarget != null)
            {
                for (var index = selectedCards.Count - 1; index >= 0; index--)
                {
                    var card = selectedCards[index];
                    slotCardAttacherTarget.AttachCard(card);
                    playerCardManager.UnSelectCard(card);
                }

                slotCardAttacherTarget = null;
            }
            else
            {
                for (var index = selectedCards.Count - 1; index >= 0; index--)
                {
                    var card = selectedCards[index];
                    slotCardAttacherFrom.AttachCard(card);
                    playerCardManager.UnSelectCard(card);
                }

                slotCardAttacherFrom = null;
            }

            selectedCards = new List<Card>();
            isSelectingCards = false;
        }
    }
}