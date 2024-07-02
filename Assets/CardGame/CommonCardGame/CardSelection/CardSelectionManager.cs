using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
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

    private InputSelectionManager inputSelectionManager;

    private void Awake()
    {
        mainCamera = Camera.main;
        inputSelectionManager = FindObjectOfType<InputSelectionManager>();
    }

    private void OnEnable()
    {
        inputSelectionManager.onCardMovementEvent += UpdateCardPosition;
        inputSelectionManager.onCardSelected += TrySelectCard;
        inputSelectionManager.onCardUnSelected += TryUnselectCard;
        inputSelectionManager.onNextCardEvent += SelectNextCard;
        inputSelectionManager.onPreviousCardEvent += SelectPreviousCard;
    }

    private void OnDisable()
    {
        inputSelectionManager.onCardMovementEvent -= UpdateCardPosition;
        inputSelectionManager.onCardSelected -= TrySelectCard;
        inputSelectionManager.onCardUnSelected -= TryUnselectCard;
        inputSelectionManager.onNextCardEvent -= SelectNextCard;
        inputSelectionManager.onPreviousCardEvent -= SelectPreviousCard;
    }


    private Vector3 pointerPosition;


    private void UpdateCardPosition(Vector2 newPosition)
    {
        pointerPosition = newPosition;
        if (!dragging || selectedCards.Count == 0) return;
        Vector3 worldMousePosition =
            mainCamera.ScreenToWorldPoint(new Vector3(newPosition.x, newPosition.y, -mainCamera.transform.position.z));
        var followCard = selectedCards.First().transform;

        followCard.position = new Vector3(worldMousePosition.x + offset.x, worldMousePosition.y + offset.y,
            followCard.position.z);
    }

    private bool isSelectingCards = false;

    private void TrySelectCard()
    {
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(pointerPosition), Vector2.zero);
        Debug.DrawRay(mainCamera.ScreenToWorldPoint(pointerPosition), Vector2.zero);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name, hit.collider.gameObject);

            List<ISelectionable> selectionables = hit.collider.GetComponentsInParent<ISelectionable>().ToList();
            foreach (var iSelectionable in selectionables)
            {
                iSelectionable.OnSelected(hit.collider.gameObject);
            }
            if (selectionables.Count != 0)
            {
                return;
            }

            Card card = hit.collider.GetComponentInParent<Card>();
            if (card == null || !card.CanBeSelected() ||selectedCards.Contains(card))
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
                    selectedCard.SetPriority(minimumPriorityOfSelectedCards + index);
                    playerCardManager.SelectCard(selectedCard);
                    selectedCard.transform.parent = card.transform;
                }

                card.onCardTriggeredCollider.AddListener(UpdateTargetSlotCardAttacher);
                Vector3 cardPosition = card.transform.position;
                Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(pointerPosition.x,
                    pointerPosition.y, -mainCamera.transform.position.z));
                offset = cardPosition - new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, cardPosition.z);
                isSelectingCards = true;
            }
        }
    }

    private void UpdateTargetSlotCardAttacher(Collider2D arg0)
    {
        slotCardAttacherTarget = arg0.GetComponentInParent<SlotCardAttacher>();
    }

    private void TryUnselectCard()
    {
        if (isSelectingCards)
        {
            dragging = false;
            var firstCard = selectedCards.First();
            var lastCard = selectedCards.Last();
            bool hasSlotCardAttacherTargetAndIsNotTheSame =
                slotCardAttacherTarget != null && slotCardAttacherFrom != slotCardAttacherTarget;
            bool isCardAttached = false;
            if (hasSlotCardAttacherTargetAndIsNotTheSame)
            {
                for (var index = 0; index < selectedCards.Count; index++)
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
                        index--;
                        isCardAttached = true;
                    }
                    else
                    {
                        Debug.Log("Card not attached" + card.ToString());
                        isCardAttached = false;
                    }
                }
                if(isCardAttached)
                slotCardAttacherTarget.ForceOnConditionMet(lastCard);

                slotCardAttacherTarget = null;
            }
            if (!hasSlotCardAttacherTargetAndIsNotTheSame || !isCardAttached)
            {
                for (var index = 0; index < selectedCards.Count; index++)
                {
                    var card = selectedCards[index];
                    slotCardAttacherFrom.AttachCard(card);
                    playerCardManager.UnSelectCard(card);
                }
                slotCardAttacherFrom.CheckSlotConditions(lastCard);
            }
            slotCardAttacherFrom = null;
            selectedCards = new List<Card>();
            isSelectingCards = false;
        }
    }

    private void SelectNextCard()
    {
        pointerPosition = Vector3.back;
    }

    private void SelectPreviousCard()
    {
        // Change the highlighted card to the previous one
        // the specific steps here depend on how you are storing/organizing your selectable cards
    }
}

public interface ISelectionable
{
    void OnSelected(GameObject selectedGameObject);
}