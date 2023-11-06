using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualUpdater : MonoBehaviour
{
    [SerializeField] List<Sprite> availableVisuals;
    [SerializeField] Card relatedCard;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite hideSprite;
    [SerializeField] VisualDataBankV2 visualDataBank;
    [SerializeField] Sprite lastShowedCard;

    public void VisualInitializer()
    {
    }

    public void OnCardTypeChanged(CardType newCardType)
    {
        Debug.Log("Card changed to " + newCardType);
        Sprite newCardSprite = visualDataBank.GetCardVisual(newCardType, relatedCard.GetCardNumber());
        UpdateCardSprite(newCardSprite);
    }

    public void OnCardNumberChanged(CardNumber newCardNumber)
    {
        Sprite newCardSprite = visualDataBank.GetCardVisual(relatedCard.GetCardType(), newCardNumber);
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

    public void OnCardUnSelected()
    {
        spriteRenderer.color = Color.white;
    }
}