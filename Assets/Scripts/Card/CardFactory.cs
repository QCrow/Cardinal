using System.Collections.Generic;
using UnityEngine;

public static class CardFactory
{
    public static Card CreateCard(GameObject cardObject, CardScriptable cardScriptable)
    {
        if (!cardObject.TryGetComponent<Card>(out var card))
        {
            throw new System.InvalidOperationException("The instantiated object does not have a Card component.");
        }

        if (cardScriptable == null)
        {
            throw new System.ArgumentNullException(nameof(cardScriptable), "Card scriptable object cannot be null.");
        }

        Dictionary<TriggerType, List<ConditionalEffect>> conditionalEffect = new();
        if (cardScriptable.HasEffect)
        {
            foreach (var serializedCondition in cardScriptable.Conditions)
            {
                if (!conditionalEffect.ContainsKey(serializedCondition.Trigger))
                {
                    conditionalEffect.Add(serializedCondition.Trigger, new List<ConditionalEffect>());
                }

                ConditionalEffect condition = CreateConditionalEffect(card, serializedCondition);
                conditionalEffect[serializedCondition.Trigger].Add(condition);
            }
        }

        card.Initialize(cardScriptable, conditionalEffect);

        return card;
    }

    public static ConditionalEffect CreateConditionalEffect(Card card, SerializableCondition condition)
    {
        List<Effect> effects = new();
        foreach (var serializedEffect in condition.Effects)
        {
            Effect effect;
            switch (serializedEffect.Keyword)
            {
                case EffectType.Apply:
                    effect = new AddModifierEffect(card, serializedEffect.Modifier, serializedEffect.Value, serializedEffect.IsTargeted, serializedEffect.Target);
                    break;
                case EffectType.Destroy:
                    effect = new DestroyEffect(card, serializedEffect.IsTargeted, serializedEffect.Target);
                    break;
                case EffectType.AddCard:
                    effect = new AddCardEffect(card, serializedEffect.CardID);
                    break;
                default:
                    throw new System.NotSupportedException($"Effect keyword '{serializedEffect.Keyword}' is not supported or implemented yet.");
            }

            if (effect == null)
            {
                throw new System.InvalidOperationException($"Effect could not be created for keyword '{serializedEffect.Keyword}'.");
            }
            effects.Add(effect);
        }

        ConditionalEffect conditionalEffect;
        switch (condition.Condition)
        {
            case ConditionType.Constant:
                conditionalEffect = new ConstantEffect(card, effects);
                break;
            case ConditionType.Position:
                conditionalEffect = new PositionCondition(card, effects, condition.Position);
                break;
            case ConditionType.TargetWithFilter:
                conditionalEffect = new TargetWithFilterCondition(card, effects, condition.TargetWithFilter, condition.Check, condition.Minimum);
                break;
            case ConditionType.Cycle:
                conditionalEffect = new CycleCondition(card, effects, condition.CycleCount);
                break;
            default:
                throw new System.NotSupportedException($"Condition type '{condition.Condition}' is not supported or implemented yet.");
        }
        return conditionalEffect;
    }
}