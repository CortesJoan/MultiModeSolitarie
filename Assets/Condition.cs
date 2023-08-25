using UnityEngine;

public abstract class Condition :ScriptableObject
{
    public abstract bool IsConditionMet();
}

public interface ICondicionable
{
}
