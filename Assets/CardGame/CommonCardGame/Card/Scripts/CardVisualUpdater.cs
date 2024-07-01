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
 

    public void OnCardTypeChanged(CardType newCardType)
    {
        UpdateCardSprite(relatedCard);
    }

    public void OnCardNumberChanged(CardNumber newCardNumber)
    {
        UpdateCardSprite(relatedCard);
    }

    void UpdateCardSprite(Card relatedCard)
    {
        Sprite newCardSprite = visualDataBank.GetCardVisual(relatedCard.GetCardType(), relatedCard.GetCardNumber());

        spriteRenderer.sprite = newCardSprite;
        lastShowedCard = newCardSprite;
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