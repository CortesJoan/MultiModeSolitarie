using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class SolvableSolitaireCardGenerator : MonoBehaviour, IGameGenerator
{
    [SerializeField] Card cardTemplatePrefab;
    [SerializeField] SolitaireLayout cardLayout;
    [SerializeField] PileOfCards pileOfCards;
    [SerializeField] GameRules gameRules;
    [SerializeField] private int randomSeed = -1;
    [SerializeField] private List<SlotCardAttacher> foundations;

    private void DestroyPreviousPileOfCards()
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

    public Card InstantiateEmptyCard()
    {
        return Instantiate<Card>(cardTemplatePrefab);
    }

    HashSet<string> existingCards = new HashSet<string>();

    public void FillVoidCards(ref List<Card> cards, int amount)
    {
        List<CardType> types = new List<CardType>()
        {
            CardType.Hearts, CardType.Tiles, CardType.Clover, CardType.Pikes
        };
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

        for (int i = 0; i < amount; i++)
        {
            CardType randomType = types[Random.Range(0, types.Count)];
            CardNumber randomNumber = numbers[Random.Range(0, numbers.Count)];
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

            existingCards.Add(cardKey);
        }
    }

    [SerializeField]
    private int
        depthLimit = 13; // This value could be adjusted, it represents the maximum "depth" the algorithm will explore.

    private bool SolveGame(int cardIndex, List<Card> cardsInTableau, List<Card> cardsInDeck,
        List<SlotCardAttacher> foundations)
    {
        if (cardsInTableau.Count == 0)
            return CheckSolved(foundations);

        // Deep copy the cards in tableau
        List<Card> tableau = new List<Card>(cardsInTableau);

        // Traversing from back
        for (int i = tableau.Count - 1; i >= 0; i--)
        {
            Card card = tableau[i];

            // Try to put the card in every possible slot and check if it leads to a solution
            foreach (SlotCardAttacher foundation in foundations)
            {
                if (foundation.TryToAttachCard(card))
                {
                    // Create new List without current card
                    List<Card> smallerTableau = new List<Card>(tableau.Where(c => c != card));

                    if (SolveGame(cardIndex, smallerTableau, cardsInDeck, foundations))
                        return true;

                    // If placing the card on the foundation leads to an unsolvable state,
                    // we detach the card and try to place it on another foundation.
                    foundation.DeAttachCard(card);
                }
            }
        }

        return false;
    }

    // Added method to check if the game is solved
    private bool CheckSolved(List<SlotCardAttacher> foundations)
    {
        foreach (SlotCardAttacher foundation in foundations)
        {
            List<Card> attachedCards = foundation.GetAttachedCards();

            // If a foundation is empty, it's not a winning state. So, Skip Checking.
            if(attachedCards.Count == 0)
                return false;

            // If a foundation's last card is not a King, it's not a winning state
            if (attachedCards.Last().GetCardNumber() != CardNumber.King)
                return false;
        }
        return true;
    }

    private bool TryMoveToAnyFoundation(Card card, List<SlotCardAttacher> foundations)
    {
        foreach (SlotCardAttacher foundation in foundations)
        {
            if (foundation.TryToAttachCard(card))
                return true;
        }
        return false;
    }

    private bool CheckWinCondition(List<SlotCardAttacher> foundations)
    {
        // Check each foundation
        foreach (SlotCardAttacher foundation in foundations)
        {
            List<Card> attachedCards = foundation.GetAttachedCards();

            // If a foundation is empty or does not start with an Ace, it's not a winning state
            if (attachedCards.Count == 0 || attachedCards[0].GetCardNumber() != CardNumber.Ace)
                return false;

            // If a foundation does not end with a King or cards are not in ascending order, it's not a winning state
            if (attachedCards[attachedCards.Count - 1].GetCardNumber() != CardNumber.King)
                return false;

            for (int i = 0; i < attachedCards.Count - 1; i++)
            {
                if ((int)attachedCards[i].GetCardNumber() + 1 != (int)attachedCards[i + 1].GetCardNumber())
                    return false;
            }
        }

        // All checks passed - it's a winning state
        return true;
    }
 
public void GenerateGame()
{
    DestroyPreviousPileOfCards();
    existingCards.Clear();

    // Generate a sorted deck of cards (King to Ace)
    List<Card> deckCards = new List<Card>();
    FillVoidCards(ref deckCards, gameRules.TotalCards);
    deckCards.Sort((x, y) => -((int)x.GetCardType() * 100 + (int)x.GetCardNumber() - ((int)y.GetCardType() * 100 + (int)y.GetCardNumber())));

    // Generate empty tableau cards
    List<List<Card>> tableauCards = new List<List<Card>>();
    cardLayout.PrepareLayout();
    int rows = cardLayout.GetRows();
    for (int i = 0; i < rows; i++)
    {
        tableauCards.Add(new List<Card>());
    }

    // Remove 1 card from the deck to the last tableau, 2 cards to the second last tableau, etc. until the first tableau has 7 cards
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < i + 1; j++)
        {
            Card card = deckCards[deckCards.Count - 1];
            deckCards.RemoveAt(deckCards.Count - 1);
            tableauCards[rows - 1 - i].Add(card);
            if (j == i)
            {
                card.Show();
            }
            else
            {
                card.Hide();
            }
        }
    }

    // the remaining deckCards forms the deck     
    pileOfCards.SetPileOfCards(deckCards);
    foreach (var card in deckCards)
    {
        card.transform.parent = pileOfCards.transform;
        card.transform.localPosition = Vector3.zero;
        card.Hide();
    }

    // Put the tableau cards into the layout
    cardLayout.PutCardsInLayout(tableauCards);

    cardLayout.ApplyChanges();
    pileOfCards.ApplyChanges();

    // Print log whether the generated deck is solvable or not
    List<Card> tCards = tableauCards.SelectMany(x => x).ToList();
    bool is_deck_solvable = SolveGame(0, tCards, deckCards, foundations);
    Debug.Log(is_deck_solvable ? $"The game with the given seed {randomSeed} is solvable." : $"The game with given seed {randomSeed} is not solvable, please try another seed.");
}
}

public static class ListExtensions
{
    public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
    {
        T element = list[oldIndex];
        list.RemoveAt(oldIndex);
        list.Insert(newIndex, element);
    }
}