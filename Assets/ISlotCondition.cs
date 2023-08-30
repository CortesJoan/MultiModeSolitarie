using System;
using UnityEngine;
 public interface ISlotCondition
{
    public bool IsConditionMet(Card cardToValue);
    public void UpdateDefaultCondition(Card cardToValue);
    
} 
