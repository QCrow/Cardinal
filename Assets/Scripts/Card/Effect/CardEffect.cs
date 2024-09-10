using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for card effects.
/// </summary>
public abstract class CardEffect
{
    public CardCondition Condition; // Reference to the condition that triggers this effect

    public int Value { get; set; }

    protected CardEffect(int value, CardCondition condition)
    {
        Value = value;
        Condition = condition;
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

    public SequenceCardEffect(int value, CardCondition condition) : base(value, condition)
    {
        Effects = new List<CardEffect>();
    }

    public SequenceCardEffect(int value, CardCondition condition, List<CardEffect> effects) : base(value, condition)
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

    protected ResourceCardEffect(int value, CardCondition condition, ResourceType resourceType) : base(value, condition)
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
    public ProduceCardEffect(int value, CardCondition condition, ResourceType resourceType) : base(value, condition, resourceType) { }

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
    public ConsumeCardEffect(int value, CardCondition condition, ResourceType resourceType) : base(value, condition, resourceType) { }

    public override bool ResolveEffect(List<Slot> targets)
    {
        ModifyResource(-Value);
        return true;
    }
}

public class DestroySelfCardEffect : CardEffect
{
    public DestroySelfCardEffect(int value, CardCondition condition) : base(value, condition) { }

    public override bool ResolveEffect(List<Slot> targets)
    {
        Condition.Card.Remove();
        return true;
    }
}

/// <summary>
/// Destroys surrounding cards to gain resources based on the number of cards destroyed.
/// </summary>
public class DestroySurroundingCardsToGainPerCardDestroyedEffect : CardEffect
{
    public enum GainType
    {
        Instant,
        PerTurn
    }

    public GainType Gain;
    public ResourceType ResourceType;

    public DestroySurroundingCardsToGainPerCardDestroyedEffect(int value, CardCondition condition, GainType gainType, ResourceType resourceType) : base(value, condition)
    {
        Gain = gainType;
        ResourceType = resourceType;
    }

    public override bool ResolveEffect(List<Slot> targets)
    {
        int totalDestroyed = 0;
        foreach (var target in Condition.Card.Slot.Neighbors)
        {
            if (target.Card)
            {
                target.Card.Remove();
                totalDestroyed++;
            }
        }

        if (totalDestroyed > 0)
        {
            if (Gain == GainType.Instant)
            {
                ResourceManager.Instance.ModifyResourceCurrentValueByAmount(ResourceType, totalDestroyed * Value);
            }
            else
            {
                CardConditionData newConditionData = new CardConditionData
                (
                    CardConditionType.Always,
                    CardEffectTriggerType.OnTurnEnd,
                    0,
                    new List<CardEffectData>
                    {
                        new CardEffectData($"Produce {ResourceType}", new List<int> { totalDestroyed * Value })
                    }
                );

                Condition.Card.AddConditionalEffect(CardEffectTriggerType.OnTurnEnd, CardFactory.CreateCondition(newConditionData, Condition.Card));
            }
        }
        return true;
    }


}
public abstract class ModifierCardEffect : CardEffect
{
    public enum TargetType
    {
        Card,
        Slot
    }

    public ModifierType _modifierType;
    public TargetType _targetType;

    protected ModifierCardEffect(int value, CardCondition condition, ModifierType modifierType, TargetType targetType) : base(value, condition)
    {
        _modifierType = modifierType;
        _targetType = targetType;
    }

    protected void ChangeModifier(int amount)
    {
        switch (_targetType)
        {
            case TargetType.Card:
                Condition.Card.AddModifier(_modifierType, amount);
                break;
            case TargetType.Slot:
                foreach (var target in Condition.Card.Slot.Neighbors)
                {
                    if (amount < 0)
                    {
                        target.RemoveModifier(_modifierType, amount);
                    }
                    else
                    {
                        target.AddModifier(_modifierType, amount);
                    }
                }
                break;
        }
    }
}

public class ApplyModifierCardEffect : ModifierCardEffect
{
    public ApplyModifierCardEffect(int value, CardCondition condition, ModifierType mod, TargetType target) : base(value, condition, mod, target) { }

    public override bool ResolveEffect(List<Slot> targets)
    {
        ChangeModifier(Value);
        return true;
    }
}

public class RemoveModifierCardEffect : ModifierCardEffect
{
    public RemoveModifierCardEffect(int value, CardCondition condition, ModifierType mod, TargetType target) : base(value, condition, mod, target) { }

    public override bool ResolveEffect(List<Slot> targets)
    {
        Debug.Log("Removing modifier with effect" + Value);
        ChangeModifier(-Value);
        return true;
    }
}