using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardSelectionManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private Camera mainCamera;
    [SerializeField] private List<Card> selectedCards;
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
        if (!context.performed || isSelectingCards)
        {
            return;
        }
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);
        Debug.DrawRay(mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name, hit.collider.gameObject);
            //todo move to interface
            PileOfCards pileOfCards = hit.collider.GetComponentInParent<PileOfCards>();
            if (pileOfCards != null)
            {
                pileOfCards.ShowNextCard();
                return;
            }
            Card card = hit.collider.GetComponentInParent<Card>();
            if (card == null || !card.CanBeSelected())
            {
                return;
            }


            SlotCardAttacher slotCardAttacherOfCurrentCard = card.GetComponentInParent<SlotCardAttacher>();
            Debug.Assert(slotCardAttacherOfCurrentCard != null, $"Invalid slotCardAttacherOfCurrentCard");
            slotCardAttacherFrom = slotCardAttacherOfCurrentCard;
            if (slotCardAttacherFrom != null)
            {
                dragging = true;
                selectedCards = slotCardAttacherFrom.GetAttachedCardsFromStartingCard(card);
                var minimumPriorityOfSelectedCards = 10;
                for (var index = 0; index < selectedCards.Count; index++)
                {
                    var selectedCard = selectedCards[index];
                    slotCardAttacherFrom.DeAttachCard(selectedCard);
                    selectedCard.SetPriority(minimumPriorityOfSelectedCards+index);
                    playerCardManager.SelectCard(selectedCard);
                    selectedCard.transform.parent = card.transform;
                }

                card.onCardTriggeredCollider.AddListener(UpdateTargetSlotCardAttacher);
                Vector3 cardPosition = card.transform.position;
                Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x,
                    mousePosition.y, -mainCamera.transform.position.z));
                offset = cardPosition - new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, cardPosition.z);
                isSelectingCards = true;
            }
        }
    }

    private void UpdateTargetSlotCardAttacher(Collider2D arg0)
    {
        slotCardAttacherTarget = arg0.GetComponentInParent<SlotCardAttacher>();
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
                    card.onCardTriggeredCollider.RemoveListener(UpdateTargetSlotCardAttacher);

                    bool isAttached = slotCardAttacherTarget.TryToAttachCard(card);
                    if (isAttached)
                    {
                        var showingCardUpdater = slotCardAttacherFrom.GetComponent<CurrentShowingCardUpdater>();

                        showingCardUpdater?.ShowNewLastCard();
                        playerCardManager.UnSelectCard(card);
                        selectedCards.Remove(card);
                        slotCardAttacherTarget = null;
                        return;
                    }
                }
            }
            for (var index = selectedCards.Count - 1; index >= 0; index--)
            {
                var card = selectedCards[index];
                slotCardAttacherFrom.AttachCard(card);
                playerCardManager.UnSelectCard(card);
            }
            slotCardAttacherFrom = null;
            selectedCards = new List<Card>();
            isSelectingCards = false;
        }
    }
}