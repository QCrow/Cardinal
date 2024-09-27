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

        ConditionalEffect conditionalEffect = null;
        if (cardScriptable.HasEffect)
        {
            conditionalEffect = CreateConditionalEffect(card, cardScriptable);
        }

        card.Initialize(cardScriptable, conditionalEffect);

        return card;
    }

    public static ConditionalEffect CreateConditionalEffect(Card card, CardScriptable cardScriptable)
    {
        Effect effect = null;
        switch (cardScriptable.Keyword)
        {
            case EffectType.Apply:
                effect = new AddModifierEffect(card, cardScriptable.Modifier, cardScriptable.Value, cardScriptable.IsTargeted, cardScriptable.Target);
                break;
            case EffectType.TempDamageUp:
                effect = new TempDamageUpEffect(card, cardScriptable.Value);
                break;
            default:
                throw new System.NotSupportedException($"Effect keyword '{cardScriptable.Keyword}' is not supported or implemented yet.");
        }

        if (effect == null)
        {
            throw new System.InvalidOperationException($"Effect is not initialized for card {cardScriptable.Name}.");
        }

        ConditionalEffect conditionalEffect;
        switch (cardScriptable.Condition)
        {
            case ConditionType.Constant:
                conditionalEffect = new ConstantEffect(card, effect);
                break;
            case ConditionType.Position:
                conditionalEffect = new PositionCondition(card, effect, cardScriptable.Position);
                break;
            case ConditionType.TargetWithProperty:
                conditionalEffect = new TargetWithPropertyCondition(card, effect, cardScriptable.TargetWithProperty, cardScriptable.Check, cardScriptable.Minimum);
                break;
            default:
                throw new System.NotSupportedException($"Condition type '{cardScriptable.Condition}' is not supported or implemented yet.");
        }
        return conditionalEffect;
    }
}