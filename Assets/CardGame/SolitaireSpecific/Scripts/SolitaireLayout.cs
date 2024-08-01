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
    public void PrepareLayout()
    {
        ClearLayout();
        int recorredSpace = 0;
        for (int i = 0; i < rows; i++)
        {
            GameObject instantiatedEmptySlot = Instantiate(emptySlotPrefab, this.transform);
            instantiatedEmptySlot.transform.localPosition = new Vector3(recorredSpace, 0, 0);
            instantiatedEmptySlot.name += i;
            layoutInstantiatedSlots.Add(instantiatedEmptySlot);

            recorredSpace += spaceBetweenRows;
        }
    }


    public void PutCardsInLayout(List<List<Card>> cards)
    {
        Debug.Log($"cards count {cards.Count}  ");
        for (var i = 0; i < layoutInstantiatedSlots.Count; i++)
        {
            SlotCardAttacher layoutSlot = layoutInstantiatedSlots[i].GetComponent<SlotCardAttacher>();
            layoutSlot.SetAttachedCardsDistance(cardYSeparation);
            if (cards.Count > i && cards[i].Count > 0)
            {
                for (int j = 0; j < cards[i].Count; j++)
                {
                    Card card = cards[i][j];
                    layoutSlot.AttachCard(card);

                }
                layoutSlot.CheckSlotConditions(cards[i].Last());
            }
        }
    }

    void ClearLayout()
    {
        foreach (var slot in layoutInstantiatedSlots)
        {
            DestroyImmediate(slot.gameObject);
        }
        layoutInstantiatedSlots.Clear();
    }

    public int GetRows()
    {
        return rows;
    }

    public bool IsPrepared()
    {
        return GetComponentsInChildren<SlotCardAttacher>().Length > 0;
    }

    public int GetTotalCardsCount()
    {
        int totalCards = 0;

        foreach (GameObject slot in layoutInstantiatedSlots)
        {
            SlotCardAttacher attacher = slot.GetComponent<SlotCardAttacher>();
            totalCards += attacher.GetAttachedCards().Count;
        }

        return totalCards;

    }
    public void ResetLayout()
    {
        for (int i = 0; i < layoutInstantiatedSlots.Count; i++)
        {
            SlotCardAttacher attacher = layoutInstantiatedSlots[i].GetComponent<SlotCardAttacher>();
           
            attacher.ResetSlot();
        }
    }
}