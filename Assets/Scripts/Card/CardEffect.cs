using System;
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
    public abstract bool ResolveEffect(List<Slot> targets);
}

/// <summary>
/// Applies a sequence of card effects in order.
/// Each effect in the sequence must be successfully resolved for the next effect to be applied.
/// Example: A SequenceCardEffect might first consume resources (ConsumeCardEffect),
/// followed by producing an output (ProduceCardEffect).
/// </summary>
public class SequenceCardEffect : CardEffect
{
    public List<CardEffect> Effects { get; set; }

    public SequenceCardEffect(int value) : base(value)
    {
        Effects = new();
    }

    public SequenceCardEffect(int value, List<CardEffect> effects) : base(value)
    {
        Effects = effects;
    }

    public override bool ResolveEffect(List<Slot> targets)
    {
        foreach (var effect in Effects)
        {
            if (!effect.ResolveEffect(targets)) return false;
        }
        return true;
    }
}

/// <summary>
/// Produces a specific type of product.
/// </summary>
public class ProduceCardEffect : CardEffect
{
    public string ProductType;

    public ProduceCardEffect(int value, string productType) : base(value)
    {
        ProductType = productType;
    }

    public override bool ResolveEffect(List<Slot> targets)
    {
        // Debug.Log($"Producing {Value} {ProductType}");
        switch (ProductType)
        {
            case "Energy":
                ResourceManager.Instance.ModifyResourceCurrentValueByAmount(ResourceType.Energy, Value);
                break;
            case "Food":
                ResourceManager.Instance.ModifyResourceCurrentValueByAmount(ResourceType.Food, Value);
                break;
            case "Morale":
                ResourceManager.Instance.ModifyResourceCurrentValueByAmount(ResourceType.Morale, Value);
                break;
            default:
                throw new NotSupportedException($"Resource '{ProductType}' is not supported or implemented yet.");
        }
        return true;
    }
}

/// <summary>
/// Consumes a specific type of product.
/// </summary>
public class ConsumeCardEffect : CardEffect
{
    public string ProductType { get; set; }

    public ConsumeCardEffect(int value, string productType) : base(value)
    {
        ProductType = productType;
    }

    public override bool ResolveEffect(List<Slot> targets)
    {
        Debug.Log($"Consuming {Value} {ProductType}");
        return true;
    }
}

