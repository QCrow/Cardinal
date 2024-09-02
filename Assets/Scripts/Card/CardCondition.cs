using System.Collections.Generic;
using UnityEngine;

public abstract class CardCondition
{
    public List<CardEffect> Effects;
    public abstract bool Validate();
    public void TriggerEffects(List<Slot> targets)
    {
        foreach (CardEffect effect in Effects)
        {
            effect.ResolveEffect(targets);
        }
    }
}

public class AlwaysCondition : CardCondition
{
    public AlwaysCondition() { }
    public override bool Validate()
    {
        return true;
    }
}

public class CountdownCondition : CardCondition
{
    public int Value;
    int CurrentValue;

    public CountdownCondition(int value)
    {
        Value = value;
        CurrentValue = value;
    }

    public override bool Validate()
    {
        CurrentValue--;
        Debug.Log($"Counting down. The count after is {CurrentValue}");
        if (CurrentValue == 0)
        {
            Debug.Log($"Resetting value to {Value}");
            CurrentValue = Value;
            return true;
        }
        return false;
    }
}