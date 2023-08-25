using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualUpdater : MonoBehaviour
{
    [SerializeField] List<Sprite> availableVisuals;
    [SerializeField] Card relatedCard;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] VisualDataBank visualDataBank;
    [SerializeField] Sprite hideSprite;

    [SerializeField] Sprite lastShowedCard;

    public void OnCardTypeChanged(CardType newCardType)
    {
        Debug.Log("Card changed to " + newCardType);
        Sprite newCardSprite = VisualDataBank.instance.GetCardVisual(newCardType, relatedCard.GetCardNumber());
        UpdateCardSprite(newCardSprite);
    }

    public void OnCardNumberChanged(CardNumber newCardNumber)
    {
        Sprite newCardSprite = VisualDataBank.instance.GetCardVisual(relatedCard.GetCardType(), newCardNumber);
        UpdateCardSprite(newCardSprite);
    }

    void UpdateCardSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
        lastShowedCard = newSprite;
    }

    public void OnHideCard()
    {
        spriteRenderer.sprite = hideSprite;
    }

    public void OnShowCard()
    {
        spriteRenderer.sprite = lastShowedCard;
        spriteRenderer.sortingOrder = 1;
    }

    public void OnPriorityDecreased(int newPriority)
    {
        spriteRenderer.sortingOrder = newPriority;
    }

    public void OnPriorityIncreased(int newPriority)
    {
        spriteRenderer.sortingOrder = newPriority;
    }

    public void OnCardSelected()
    {
        spriteRenderer.color = Color.yellow;
    }
}