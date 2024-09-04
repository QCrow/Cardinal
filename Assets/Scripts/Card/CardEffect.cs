using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for card effects.
/// </summary>
public abstract class CardEffect
{
    public int Value { get; set; }

    protected CardEffect(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Resolves the effect on the target slots.
    /// </summary>
    /// <param name="targets">The list of target slots on which to apply the effect.</param>
    /// <returns>True if the effect was successfully resolved; otherwise, false.</returns>
    public abstract bool ResolveEffect(List<Slot> targets);
}

/// <summary>
/// Applies a sequence of card effects in order.
/// Each effect in the sequence must be successfully resolved for the next effect to be applied.
/// </summary>
public class SequenceCardEffect : CardEffect
{
    public List<CardEffect> Effects { get; set; }

    public SequenceCardEffect(int value) : base(value)
    {
        Effects = new List<CardEffect>();
    }

    public SequenceCardEffect(int value, List<CardEffect> effects) : base(value)
    {
        Effects = effects ?? new List<CardEffect>();
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
/// Base class for effects that modify resources.
/// </summary>
public abstract class ResourceCardEffect : CardEffect
{
    public ResourceType ResourceType { get; set; }

    protected ResourceCardEffect(int value, ResourceType resourceType) : base(value)
    {
        ResourceType = resourceType;
    }

    protected void ModifyResource(int amount)
    {
        ResourceManager.Instance.ModifyResourceCurrentValueByAmount(ResourceType, amount);
    }
}

/// <summary>
/// Produces a specific type of product.
/// </summary>
public class ProduceCardEffect : ResourceCardEffect
{
    public ProduceCardEffect(int value, ResourceType resourceType) : base(value, resourceType) { }

    public override bool ResolveEffect(List<Slot> targets)
    {
        ModifyResource(Value);
        return true;
    }
}

/// <summary>
/// Consumes a specific type of product.
/// </summary>
public class ConsumeCardEffect : ResourceCardEffect
{
    public ConsumeCardEffect(int value, ResourceType resourceType) : base(value, resourceType) { }

    public override bool ResolveEffect(List<Slot> targets)
    {
        ModifyResource(-Value);
        return true;
    }
}
