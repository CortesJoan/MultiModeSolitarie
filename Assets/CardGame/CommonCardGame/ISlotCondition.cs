using System;
using UnityEngine;
 public interface ISlotCondition
{
    public ISlotCondition Clone();

    public bool IsConditionMet(Card cardToValue);

    public void OnConditionIsMet(Card cardToValue);
    public void UpdateDefaultCondition(Card cardToValue);
    
} 
