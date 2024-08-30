using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for card effects.
/// </summary>
public abstract class CardEffect
{
    public int Value;

    public CardEffect(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Resolves the effect on the target slots.
    /// </summary>
    public abstract bool ResolveEffect(List<Slot> targetSlots);
}

/// <summary>
/// Applies a sequence of card effects in order.
/// Each effect in the sequence must be successfully resolved for the next effect to be applied.
/// Example: A SequenceEffect might first consume resources (ConsumeEffect),
/// followed by producing an output (ProduceEffect).
/// </summary>
public class SequenceEffect : CardEffect
{
    public List<CardEffect> Effects { get; set; }

    public SequenceEffect(int value) : base(value)
    {
        Effects = new();
    }

    public SequenceEffect(int value, List<CardEffect> effects) : base(value)
    {
        Effects = effects;
    }

    public override bool ResolveEffect(List<Slot> targetSlots)
    {
        foreach (var effect in Effects)
        {
            if (!effect.ResolveEffect(targetSlots)) return false;
        }
        return true;
    }
}

/// <summary>
/// Produces a specific type of product.
/// </summary>
public class ProduceEffect : CardEffect
{
    public string ProductType { get; set; }

    public ProduceEffect(int value, string productType) : base(value)
    {
        ProductType = productType;
    }

    public override bool ResolveEffect(List<Slot> targetSlots)
    {
        Debug.Log($"Producing {Value} {ProductType}");
        return true;
    }
}

/// <summary>
/// Consumes a specific type of product.
/// </summary>
public class ConsumeEffect : CardEffect
{
    public string ProductType { get; set; }

    public ConsumeEffect(int value, string productType) : base(value)
    {
        ProductType = productType;
    }

    public override bool ResolveEffect(List<Slot> targetSlots)
    {
        Debug.Log($"Consuming {Value} {ProductType}");
        return true;
    }
}
