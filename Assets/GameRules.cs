using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new GameRules", menuName = "GAMERULES", order = 100)]

public class GameRules : ScriptableObject
{
    [SerializeField] int maxHoldingCardsNumber;
    [SerializeField] int cardsToPutInLayout;
    [SerializeField] int totalCards;

    public int TotalCards { get => totalCards; set => totalCards = value; }
}
