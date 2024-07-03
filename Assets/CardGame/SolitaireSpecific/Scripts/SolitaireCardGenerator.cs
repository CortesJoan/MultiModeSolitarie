using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolitaireCardGenerator : MonoBehaviour, IGameGenerator
{
    [SerializeField] Card cardTemplatePrefab;
    [SerializeField] SolitaireLayout cardLayout;
    [SerializeField] PileOfCards pileOfCards;
    [SerializeField] GameRules gameRules;
    [SerializeField] private int randomSeed = -1;


    [ContextMenu(nameof(GenerateGame))]
    public void GenerateGame()
    {
        DestroyPreviousPileOfCards();
        List<List<Card>> generatedCards = new List<List<Card>>();
        int remainingCards = gameRules.TotalCards;
        existingCards.Clear();
        cardLayout.PrepareLayout();
        int rows = cardLayout.GetRows();

        int cardsPerRow = 1;
        for (int i = 0; i < rows; i++)
        {
            List<Card> currentRowCards = new List<Card>();
            FillVoidCards(ref currentRowCards, cardsPerRow);
            cardsPerRow++;
            generatedCards.Add(currentRowCards);
            for (var index = 0; index < currentRowCards.Count - 1; index++)
            {
                var c = currentRowCards[index];
                c.Hide();
                c.ToggleSelectable(false);
                c.IncreasePriority(index + 1);
            }
            var lastCard = currentRowCards[^1];
            lastCard.Show();
            lastCard.ToggleSelectable(true);
            lastCard.SetMaxPriority();

            remainingCards -= cardsPerRow;
        }

        cardLayout.PutCardsInLayout(generatedCards);

        List<Card> cardsToAddToThePile = new List<Card>();
        FillVoidCards(ref cardsToAddToThePile, remainingCards);
        pileOfCards.SetPileOfCards(cardsToAddToThePile);
        foreach (var card in cardsToAddToThePile)
        {
            card.transform.parent = pileOfCards.transform;
            card.transform.localPosition = Vector3.zero;
            card.RestorePriority();
            card.Hide();
        }
        cardLayout.ApplyChanges();
        pileOfCards.ApplyChanges();
    }

    public Card InstantiateEmptyCard()
    {
        return Instantiate<Card>(cardTemplatePrefab);
    }

    HashSet<string> existingCards = new HashSet<string>();

    public void FillVoidCards(ref List<Card> cards, int amount)
    {
        List<CardType> types = new List<CardType>()
            { CardType.Hearts, CardType.Tiles, CardType.Clover, CardType.Pikes };
        List<CardNumber> numbers = new List<CardNumber>()
        {
            CardNumber.Ace, CardNumber.Number2, CardNumber.Number3, CardNumber.Number4, CardNumber.Number5,
            CardNumber.Number6, CardNumber.Number7, CardNumber.Number8, CardNumber.Number9, CardNumber.Number10,
            CardNumber.Jack, CardNumber.Queen, CardNumber.King
        };


        foreach (Card card in cards)
        {
            existingCards.Add(card.GetCardType().ToString() + card.GetCardNumber().ToString());
        }

        if (randomSeed == -1)
        {
            randomSeed = UnityEngine.Random.seed;
        }
        else
        {
            UnityEngine.Random.InitState(randomSeed);
        }
        for (int i = 0; i < amount; i++)
        {
            CardType randomType = types[Random.Range(0, types.Count)];
            CardNumber randomNumber = numbers[Random.Range(1, numbers.Count)];

            string cardKey = randomType.ToString() + randomNumber.ToString();
            while (existingCards.Contains(cardKey))
            {
                randomType = types[Random.Range(0, types.Count)];
                randomNumber = numbers[Random.Range(0, numbers.Count)];
                cardKey = randomType.ToString() + randomNumber.ToString();
            }

            Card newCard = InstantiateEmptyCard();
            newCard.SetCardType(randomType);
            newCard.SetCardNumber(randomNumber);
            cards.Add(newCard);
            newCard.gameObject.name = cardKey;
            existingCards.Add(cardKey);
        }
    }

    public void DestroyPreviousPileOfCards()
    {
        var pileOfCardsCards = pileOfCards.GetPileOfCards();
        for (var i = 0; i < pileOfCardsCards.Count; i++)
        {
            var cards = pileOfCardsCards[i];
            if (cards != null)
            {
                this.DestroyGameObjectInAnyContext(cards.gameObject);
            }
        }
        pileOfCardsCards.Clear();
    }
}