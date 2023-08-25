using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolitaireLayout : MonoBehaviour
{
    [SerializeField] int rows = 7;
    [SerializeField] int spaceBetweenRows = 2;
    [SerializeField] GameObject emptySlotPrefab;
    [SerializeField] List<GameObject> layoutInstantiatedSlots;
    [SerializeField] float cardYSeparation = 1;

    [ContextMenu(nameof(PrepareLayout))]
    void PrepareLayout()
    {
        foreach (var slot in layoutInstantiatedSlots)
        {
            DestroyImmediate(slot.gameObject);
        }
        layoutInstantiatedSlots.Clear();
        int recorredSpace = 0;
        for (int i = 0; i < rows; i++)
        {
            GameObject instantiatedEmptySlot = Instantiate(emptySlotPrefab, this.transform);
            instantiatedEmptySlot.transform.localPosition = new Vector3(recorredSpace, 0, 0);

            layoutInstantiatedSlots.Add(instantiatedEmptySlot);

            recorredSpace += spaceBetweenRows;
        }
    }

    public void PutCardsInLayout(List<List<Card>> cards)
    {
        Debug.Log($"cards count {cards.Count}  ");

        for (int i = 0; i < layoutInstantiatedSlots.Count; i++)
        {
            GameObject layoutSlot = layoutInstantiatedSlots[i];
            float currentSeparation = 0;
            foreach (Card card in cards[i])
            {
                card.transform.SetParent(layoutSlot.transform);
                card.transform.localPosition = Vector3.down * currentSeparation;
                currentSeparation += cardYSeparation;
            }
         }
    }
    public int GetRows()
    {
        return rows;
    }
}
