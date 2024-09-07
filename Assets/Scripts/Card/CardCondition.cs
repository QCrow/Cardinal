using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class representing a condition that must be met to trigger card effects.
/// </summary>
public abstract class CardCondition
{
    // List of effects that will be triggered when the condition is validated
    private List<CardEffect> _effects = new();

    /// <summary>
    /// Abstract method to validate if the condition is met.
    /// Derived classes must implement this to specify the condition's logic.
    /// </summary>
    /// <returns>True if the condition is met, otherwise false.</returns>
    public abstract bool Validate();

    /// <summary>
    /// Triggers all effects associated with this condition on the provided targets.
    /// </summary>
    /// <param name="targets">The list of targets on which the effects will be resolved.</param>
    public void TriggerEffects(List<Slot> targets)
    {
        // Check if Effects is null or empty before attempting to trigger effects
        if (_effects == null || _effects.Count == 0)
        {
            Debug.LogWarning("No effects to trigger. The Effects list is either null or empty.");
            return;
        }

        foreach (CardEffect effect in _effects)
        {
            effect.ResolveEffect(targets);
        }
    }

    /// <summary>
    /// Adds an effect to the list of effects associated with this condition.
    /// </summary>
    /// <param name="effect">The effect to add to the list.</param>
    public void AddEffect(CardEffect effect)
    {
        _effects.Add(effect);
    }
}

/// <summary>
/// A condition that always returns true, effectively always triggering the associated effects.
/// </summary>
public class AlwaysCondition : CardCondition
{
    // Constructor for AlwaysCondition
    public AlwaysCondition() { }

    /// <summary>
    /// Always returns true, meaning this condition is always met.
    /// </summary>
    /// <returns>True</returns>
    public override bool Validate()
    {
        return true;
    }
}

/// <summary>
/// A condition that decrements a counter each time it is validated. 
/// When the counter reaches zero, it resets and the condition is met.
/// </summary>
public class CountdownCondition : CardCondition
{
    public int Value;  // Initial countdown value
    public int CurrentValue;  // Current countdown state

    /// <summary>
    /// Initializes the countdown condition with a specific value.
    /// </summary>
    /// <param name="value">The initial value for the countdown.</param>
    public CountdownCondition(int value)
    {
        Value = value;
        CurrentValue = value;
    }

    /// <summary>
    /// Decrements the countdown. When it reaches zero, resets the countdown and returns true.
    /// </summary>
    /// <returns>True if the countdown reaches zero, otherwise false.</returns>
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

public class ClusterCondition : CardCondition
{
    public int Value;
    public ClusterCondition(int value)
    {
        Value = value;
    }

    public override bool Validate()
    {
        return false;
    }

    public bool Validate(int row, int col)
    {
        int clusterSize = EffectResolveManager.Instance.Board.GetClusterSize(row, col, new HashSet<(int, int)>());
        Debug.Log($"Cluster size for {row},{col} is {clusterSize}");
        if (clusterSize >= Value)
        {
            return true;
        }
        return false;
    }
}
