 
using System.Collections.Generic;
using System.Linq;
using UnityEngine; 
public class SolitaireCardGenerator : MonoBehaviour, IGameGenerator
{
    [SerializeField] private Card cardTemplatePrefab;
    [SerializeField] private SolitaireLayout cardLayout;
    [SerializeField] private PileOfCards pileOfCards;
    [SerializeField] private GameRules gameRules;
    [SerializeField] private int randomSeed = -1;
    [SerializeField] private SlotCardAttacher[] foundationSlotAttachers;
    public enum GenerationMode
    {
        Random,
        Solvable
    }

    [SerializeField] private GenerationMode generationMode = GenerationMode.Random;

    // Data structure to store solvable level moves
    [System.Serializable]
    public class SolvableMove
    {
        public enum MoveType { TableauToTableau, TableauToFoundation, PileToTableau, PileToFoundation }
        public MoveType moveType;
        public int sourcePileIndex; // Index of source pile (tableau or stock)
        public int sourceCardIndex; // Index of card in source pile
        public int destinationPileIndex; // Index of destination pile 
    }

    // Wrapper class for solvable levels
    [System.Serializable]
    public class SolvableLevel
    {
        public List<SolvableMove> moves = new List<SolvableMove>();
    }

    // Example solvable level data - replace with your actual levels
    [SerializeField] private List<SolvableLevel> solvableLevels = new List<SolvableLevel>();

    [ContextMenu(nameof(GenerateGame))]
    public void GenerateGame()
    {
        switch (generationMode)
        {
            case GenerationMode.Random:
                GenerateRandomGame();
                break;
            case GenerationMode.Solvable:
                GenerateSolvableGame();
                break;
            default:
                Debug.LogError("Invalid Generation Mode selected.");
                break;
        }
    }

    private void GenerateRandomGame()
    {
        DestroyPreviousPileOfCards();
        ResetFoundations();

        List<List<Card>> generatedCards = new List<List<Card>>();
        int remainingCards = gameRules.TotalCards;
        existingCards.Clear();
        cardLayout.PrepareLayout();
        int rows = cardLayout.GetRows();

        int cardsPerRow = 1;
        for (int i = 0; i < rows; i++)
        {
            List<Card> currentRowCards = new List<Card>();
            FillVoidCardsRandomly(ref currentRowCards, cardsPerRow);
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
        FillVoidCardsRandomly(ref cardsToAddToThePile, remainingCards);
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
    private void ResetFoundations()
    {
        for (int i = 0; i < foundationSlotAttachers.Length; i++)
        {
            foundationSlotAttachers[i].ResetSlot();
        }
    }
    public void Clear()
    {
        DestroyPreviousPileOfCards();
        ResetFoundations();
        cardLayout.PrepareLayout();
    }
    private void GenerateSolvableGame()
    {
        Debug.Log("---- Starting GenerateSolvableGame ----");
        DestroyPreviousPileOfCards();
        ResetFoundations();
        // 1. Initialize data structures
        List<Card> deck = GenerateFullDeck();
        List<List<Card>> tableau = CreateEmptyTableau();
        List<Card> stockPile = new List<Card>();

        // 2. Shuffle the deck
        ShuffleCards(deck);

        // 3. Parameters for constraint-based generation (adjust these for difficulty)
        int maxBlockingCards = 2;
        int acesDeucesWeight = 5;

        // 4. Distribute cards to tableau and foundations
        int cardIndex = 0;
        int cardsPerRow = 1;
        for (int i = 0; i < tableau.Count; i++)
        {
            int blockingCardsInColumn = 0;
            for (int j = 0; j < cardsPerRow; j++)
            {
                if (cardIndex > deck.Count) break;

                Card card = deck[cardIndex++];

                bool placedOnFoundation = TryPlaceOnFoundation(card);

                if (!placedOnFoundation)
                {
                    // 4b. Constraint: Limit blocking cards in the tableau
                    if (j < cardsPerRow - 1 && IsBlockingCard(card, tableau[i]))
                    {
                        if (blockingCardsInColumn < maxBlockingCards)
                        {
                            blockingCardsInColumn++;
                        }
                        else
                        {
                            // Find a non-blocking card
                            int k = cardIndex;
                            while (k < deck.Count && IsBlockingCard(deck[k], tableau[i]))
                            {
                                k++;
                            }

                            if (k < deck.Count)
                            {
                                card = deck[k];
                                (deck[cardIndex - 1], deck[k]) = (deck[k], deck[cardIndex - 1]); // Swap 
                            }
                        }
                    }

                    // 4c. Constraint: Prioritize Aces and Deuces
                    if (card.GetCardNumber() == CardNumber.Ace || card.GetCardNumber() == CardNumber.Number2)
                    {
                        cardIndex -= acesDeucesWeight;
                        if (cardIndex < 0) cardIndex = 0;
                    }

                    tableau[i].Add(card);
                }
            }
            cardsPerRow++;
        }

        // 5. Remaining cards go to the stock pile
        while (cardIndex < deck.Count)
        {
            stockPile.Add(deck[cardIndex++]);
        }

        // 6. Reset and Prepare the Layout:
        cardLayout.ResetLayout();
        cardLayout.PrepareLayout();

        // 7. Place all tableau cards: 
        cardLayout.PutCardsInLayout(tableau);

        // 8. Set up the stock pile:
        pileOfCards.SetPileOfCards(stockPile);

        // 9. Set up tableau and stock pile cards 
        //   (this logic is copied from GenerateRandomGame)
       int  remainingCards = gameRules.TotalCards;
        cardsPerRow = 1;
        for (int i = 0; i < tableau.Count; i++)
        {
            for (var index = 0; index < tableau[i].Count - 1; index++)
            {
                var c = tableau[i][index];
                c.Hide();
                c.ToggleSelectable(false);
                c.IncreasePriority(index + 1);
            }

            if (tableau[i].Count > 0)
            {
                var lastCard = tableau[i][tableau[i].Count - 1];
                lastCard.Show();
                lastCard.ToggleSelectable(true);
                lastCard.SetMaxPriority();
            }

            remainingCards -= cardsPerRow;
            cardsPerRow++;
        }

        foreach (var card in stockPile)
        {
            card.transform.parent = pileOfCards.transform;
            card.transform.localPosition = Vector3.zero;
            card.RestorePriority();
            card.Hide();
        }

        cardLayout.ApplyChanges();
        pileOfCards.ApplyChanges();
        Debug.Log("---- Finished GenerateSolvableGame ----");
    }

    // --- Helper Methods ---
    private bool TryPlaceOnFoundation(Card card)
    {
        card.name = card.ToString();
        int foundationIndex = (int)card.GetCardType();
        Debug.Log($"Trying to place {card.name} on foundation {foundationIndex}"); // Log the attempt

        if (foundationSlotAttachers[foundationIndex].TryToAttachCard(card))
        {
            Debug.Log($"  -> Successfully placed {card.ToString()} on foundation!"); // Log success
            return true;
        }

        Debug.Log($"  -> Could not place {card.ToString()} on foundation."); // Log failure
        return false;
    }

    private bool IsBlockingCard(Card card, List<Card> column)
    {
        if (column.Count == 0) return false;

        Card lastCard = column.Last();

        return (int)card.GetCardNumber() < (int)lastCard.GetCardNumber() &&
               card.GetCardColor() != lastCard.GetCardColor();
    }

    private List<Card> GenerateFullDeck()
    {
        List<Card> deck = new List<Card>();
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            foreach (CardNumber number in System.Enum.GetValues(typeof(CardNumber)))
            {
                if (number != CardNumber.Joker && number != CardNumber.Any)
                {
                    Card newCard = InstantiateEmptyCard();
                    newCard.SetCardType(type);
                    newCard.SetCardNumber(number);
                    deck.Add(newCard);
                }
            }
        }
        return deck;
    }

    private void ShuffleCards(List<Card> cards)
    {
        if (randomSeed == -1)
        {
            randomSeed = UnityEngine.Random.seed;
        }
        else
        {
            UnityEngine.Random.InitState(randomSeed);
        }

        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]); // Swap cards
        }
    }

    private void FillVoidCardsRandomly(ref List<Card> cards, int amount)
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

    public Card InstantiateEmptyCard()
    {
        return Instantiate<Card>(cardTemplatePrefab);
    }

    private HashSet<string> existingCards = new HashSet<string>();

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

    private List<List<Card>> CreateEmptyTableau()
    {
        List<List<Card>> emptyTableau = new List<List<Card>>();
        for (int i = 0; i < cardLayout.GetRows(); i++)
        {
            emptyTableau.Add(new List<Card>());
        }
        return emptyTableau;
    }

    private List<List<Card>> CreateEmptyFoundations()
    {
        List<List<Card>> emptyFoundations = new List<List<Card>>();
        for (int i = 0; i < 4; i++)
        {
            emptyFoundations.Add(new List<Card>());
        }
        return emptyFoundations;
    }

    private void ReverseMove(SolvableMove move, List<List<Card>> tableau, List<Card> stockPile, List<List<Card>> foundations)
    {
        Card cardToMove;
        switch (move.moveType)
        {
            case SolvableMove.MoveType.TableauToTableau:
                cardToMove = tableau[move.destinationPileIndex].Last();
                tableau[move.destinationPileIndex].RemoveAt(tableau[move.destinationPileIndex].Count - 1);
                tableau[move.sourcePileIndex].Insert(move.sourceCardIndex, cardToMove);
                break;

            case SolvableMove.MoveType.TableauToFoundation:
                cardToMove = foundations[move.destinationPileIndex].Last();
                foundations[move.destinationPileIndex].RemoveAt(foundations[move.destinationPileIndex].Count - 1);
                tableau[move.sourcePileIndex].Insert(move.sourceCardIndex, cardToMove);
                break;

            case SolvableMove.MoveType.PileToTableau:
                cardToMove = stockPile.Last();
                stockPile.RemoveAt(stockPile.Count - 1);
                // Correction: Insert at the END of the destination tableau pile
                tableau[move.destinationPileIndex].Add(cardToMove);
                break;

            case SolvableMove.MoveType.PileToFoundation:
                cardToMove = foundations[move.destinationPileIndex].Last();
                foundations[move.destinationPileIndex].RemoveAt(foundations[move.destinationPileIndex].Count - 1);
                stockPile.Add(cardToMove);
                break;
        }
    }
    // Helper method to log the current game state 
    private void LogGameState(List<List<Card>> tableau, List<Card> stockPile, List<List<Card>> foundations)
    {
        Debug.Log("---- Current Game State ----");
        Debug.Log("Tableau:");
        for (int i = 0; i < tableau.Count; i++)
        {
            Debug.Log($"  Column {i}: {string.Join(", ", tableau[i].Select(card => card.name))}");
        }

        Debug.Log("Stock Pile:");
        Debug.Log($"  {string.Join(", ", stockPile.Select(card => card.name))}");

        Debug.Log("Foundations:");
        for (int i = 0; i < foundations.Count; i++)
        {
            Debug.Log($"  Foundation {i}: {string.Join(", ", foundations[i].Select(card => card.name))}");
        }
        Debug.Log("------------------------");
    }
}