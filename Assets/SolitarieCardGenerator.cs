using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolitarieCardGenerator : MonoBehaviour, IGameGenerator
{
    [SerializeField] Card cardTemplatePrefab;
    [SerializeField] SolitaireLayout cardLayout;
    [SerializeField] PileOfCards pileOfCards;
    [SerializeField] GameRules gameRules;

    [ContextMenu(nameof(GenerateGame))]
    public void GenerateGame()
    {
        DestroyPreciousPileOfCards();
        if (VisualDataBank.instance == null)
        {
            VisualDataBank.instance = FindObjectOfType<VisualDataBank>();
        }
        List<List<Card>> generatedCards = new List<List<Card>>();
        int remainingCards = gameRules.TotalCards;
        existingCards.Clear();
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
            }
            currentRowCards[^1].Show(); 
            currentRowCards[^1].SetMaxPriority();

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
            card.Hide();
        }
    }

    public Card InstantiateEmptyCard()
    {
        return Instantiate<Card>(cardTemplatePrefab);
    }

    HashSet<string> existingCards = new HashSet<string>();

    public void FillVoidCards(ref List<Card> cards, int amount)
    {
        // Create a list of all possible card types and numbers

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

        // Loop through the list and fill the void cards with random cards that are not already in the list
        for (int i = 0; i < amount; i++)
        {
            // Pick a random card type and number
            CardType randomType = types[Random.Range(0, types.Count)];
            CardNumber randomNumber = numbers[Random.Range(1, numbers.Count)];

            // Check if the card already exists in the list
            string cardKey = randomType.ToString() + randomNumber.ToString();
            while (existingCards.Contains(cardKey))
            {
                // If it does, pick another random card
                randomType = types[Random.Range(0, types.Count)];
                randomNumber = numbers[Random.Range(0, numbers.Count)];
                cardKey = randomType.ToString() + randomNumber.ToString();
            }

            // Create a new card with the random type and number and add it to the list
            Card newCard = InstantiateEmptyCard();
            newCard.SetCardType(randomType);
            newCard.SetCardNumber(randomNumber);
            cards.Add(newCard);

            // Add the card to the hash set
            existingCards.Add(cardKey);
        }
    }

    public void DestroyPreciousPileOfCards()
    {
        var pileOfCardsCards = pileOfCards.GetPileOfCards();
        for (var i = 0; i < pileOfCardsCards.Count; i++)
        {
            var cards = pileOfCardsCards[i];
#if UNITY_EDITOR
            DestroyImmediate(cards.gameObject);
#else
            Destroy(cards.gameObject);
#endif
        }
        pileOfCardsCards.Clear();
    }
}

public interface IGameGenerator
{
    void GenerateGame();
}