// using System;
// using System.Collections.Generic;
// using UnityEngine;

// /// <summary>
// /// Instantiates a Card gameobject based on JSON or Scriptable Object data.
// /// </summary>
// public static class CardFactory
// {
//     /// <summary>
//     /// Creates a card gameobject from the given prefab and data.
//     /// </summary>
//     /// <param name="cardPrefab">The prefab to instantiate.</param>
//     /// <param name="data">The data to initialize the card with.</param>
//     /// <returns>The instantiated card gameobject.</returns>
//     public static GameObject CreateCard(GameObject cardPrefab, CardData data)
//     {
//         if (cardPrefab == null)
//         {
//             throw new ArgumentNullException(nameof(cardPrefab), "Card prefab cannot be null.");
//         }

//         GameObject cardObject = GameObject.Instantiate(cardPrefab);
//         Card card = cardObject.GetComponent<Card>();

//         if (card == null)
//         {
//             throw new InvalidOperationException("The instantiated object does not have a Card component.");
//         }

//         CreateCard(card, data);

//         return cardObject;
//     }


//     /// <summary>
//     /// Initializes a pre-instantiated card from the given data.
//     /// </summary>
//     /// <param name="card">The card object to initialize.</param>
//     /// <param name="data">The data to initialize the card with.</param>
//     public static void CreateCard(Card card, CardData data)
//     {
//         // Create a dictionary to map triggers to conditions
//         Dictionary<CardEffectTriggerType, List<CardCondition>> conditions = new();

//         foreach (var conditionData in data.ConditionalEffects)
//         {
//             CardCondition condition = CreateCondition(conditionData, card);

//             // Add the condition to the corresponding trigger in the dictionary
//             if (!conditions.ContainsKey(conditionData.TriggerType))
//             {
//                 conditions[conditionData.TriggerType] = new List<CardCondition>();
//             }
//             conditions[conditionData.TriggerType].Add(condition);
//         }

//         // Initialize the card based on its type
//         if (card is BuildingCard buildingCard && data is BuildingCardData buildingCardData)
//         {
//             buildingCard.Initialize(buildingCardData.CardID, buildingCardData.CardName, conditions, buildingCardData.ValidTargets, buildingCardData.Traits);
//         }
//         else
//         {
//             throw new InvalidCastException($"Expected BuildingCardData, but got {data.GetType().Name}");
//         }
//     }

//     /// <summary>
//     /// Creates a card condition based on the provided data.
//     /// </summary>
//     /// <param name="conditionData">The data defining the condition.</param>
//     /// <returns>The created CardCondition object.</returns>
//     public static CardCondition CreateCondition(CardConditionData conditionData, Card card)
//     {
//         CardCondition condition;

//         switch (conditionData.ConditionType)
//         {
//             case CardConditionType.Always:
//                 condition = new AlwaysCondition();
//                 break;
//             case CardConditionType.Countdown:
//                 condition = new CountdownCondition(conditionData.ConditionValue);
//                 break;
//             case CardConditionType.Cluster:
//                 condition = new ClusterCondition(conditionData.ConditionValue);
//                 break;
//             default:
//                 throw new NotSupportedException($"Condition type '{conditionData.ConditionType}' is not supported.");
//         }
//         condition.Card = card;
//         // Add effects to the condition
//         foreach (CardEffectData effectData in conditionData.Effects)
//         {
//             condition.AddEffect(CreateEffect(effectData, condition));
//         }

//         return condition;
//     }


//     /// <summary>
//     /// Creates a card effect based on the provided data.
//     /// </summary>
//     /// <param name="effectData">The data defining the effect.</param>
//     /// <returns>The created CardEffect object.</returns>
//     public static CardEffect CreateEffect(CardEffectData effectData, CardCondition condition)
//     {
//         // Split the effect keyword into tokens to determine the effect type and parameters
//         List<string> tokens = new(effectData.Keyword.Split(' '));

//         // Ensure there is at least one token to process
//         if (tokens.Count < 1)
//         {
//             throw new ArgumentException("Effect data keyword is incomplete.");
//         }

//         string effectType = tokens[0];

//         // Handle different cases based on the effect type
//         return effectType switch
//         {
//             "Produce" => HandleResourceEffect(tokens, effectData, condition),
//             "Consume" => HandleResourceEffect(tokens, effectData, condition),
//             "DestroySurroundingToGain" => HandleDestroySurroundingToGainEffect(tokens, effectData, condition),
//             "DestroySelf" => new DestroySelfCardEffect(effectData.Values[0], condition),
//             "Apply" => HandleModifierEffect(tokens, effectData, condition),
//             "Remove" => HandleModifierEffect(tokens, effectData, condition),
//             _ => throw new NotSupportedException($"Effect keyword '{effectType}' is not supported or implemented yet.")
//         };
//     }

//     public static CardEffect CreateCounterEffect(CardEffect effect)
//     {
//         switch (effect)
//         {
//             case ApplyModifierCardEffect applyModifierEffect:
//                 return new RemoveModifierCardEffect(applyModifierEffect.Value, applyModifierEffect.Condition, applyModifierEffect._modifierType, applyModifierEffect._targetType);
//             default:
//                 throw new NotSupportedException($"Counter effect for '{effect.GetType().Name}' is not supported or implemented yet.");
//         }
//     }


//     // Helper method to handle resource-based effects (Produce, Consume, etc.)
//     private static CardEffect HandleResourceEffect(List<string> tokens, CardEffectData effectData, CardCondition condition)
//     {
//         if (tokens.Count < 2)
//         {
//             throw new ArgumentException("Resource-based effect requires a resource type.");
//         }

//         string resourceTypeString = tokens[1];

//         if (!Enum.TryParse(resourceTypeString, out ResourceType resourceType))
//         {
//             throw new InvalidCastException($"Failed to parse ResourceType from '{resourceTypeString}'");
//         }

//         return tokens[0] switch
//         {
//             "Produce" => new ProduceCardEffect(effectData.Values[0], condition, resourceType),
//             "Consume" => new ConsumeCardEffect(effectData.Values[0], condition, resourceType),
//             _ => throw new NotSupportedException($"Resource-based effect type '{tokens[0]}' is not supported or implemented yet.")
//         };
//     }

//     private static CardEffect HandleDestroySurroundingToGainEffect(List<string> tokens, CardEffectData effectData, CardCondition condition)
//     {
//         if (tokens.Count < 3)
//         {
//             throw new ArgumentException("DestroySurroundingToGain effect requires a gain type and resource type.");
//         }

//         if (!Enum.TryParse(tokens[1], out DestroySurroundingCardsToGainPerCardDestroyedEffect.GainType gainType))
//         {
//             throw new InvalidCastException($"Failed to parse GainType from '{tokens[1]}'");
//         }

//         if (!Enum.TryParse(tokens[2], out ResourceType resourceType))
//         {
//             throw new InvalidCastException($"Failed to parse ResourceType from '{tokens[2]}'");
//         }

//         return new DestroySurroundingCardsToGainPerCardDestroyedEffect(effectData.Values[0], condition, gainType, resourceType);
//     }

//     private static CardEffect HandleModifierEffect(List<string> tokens, CardEffectData effectData, CardCondition condition)
//     {
//         if (tokens.Count < 3)
//         {
//             throw new ArgumentException("Modifier effect requires a modifier type and target type.");
//         }

//         if (!Enum.TryParse(tokens[1], out ModifierType modifierType))
//         {
//             throw new InvalidCastException($"Failed to parse ModifierType from '{tokens[1]}'");
//         }

//         if (!Enum.TryParse(tokens[2], out ModifierCardEffect.TargetType targetType))
//         {
//             throw new InvalidCastException($"Failed to parse ModifierTargetType from '{tokens[2]}'");
//         }

//         return
//             tokens[0] switch
//             {
//                 "Apply" => new ApplyModifierCardEffect(effectData.Values[0], condition, modifierType, targetType),
//                 "Remove" => new RemoveModifierCardEffect(effectData.Values[0], condition, modifierType, targetType),
//                 _ => throw new NotSupportedException($"Modifier effect type '{tokens[0]}' is not supported or implemented yet.")
//             };
//     }
// }

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
            case EffectKeyword.Apply:
                effect = new AddModifierEffect(card, cardScriptable.Modifier, cardScriptable.Value, cardScriptable.IsTargeted, cardScriptable.Target);
                break;
            case EffectKeyword.DamageUp:
                effect = new TempDamageUpEffect(card, cardScriptable.Value);
                break;
            default:
                throw new System.NotSupportedException($"Effect keyword '{cardScriptable.Keyword}' is not supported or implemented yet.");
        }

        if (effect == null)
        {
            throw new System.InvalidOperationException($"Effect is not initialized for card {cardScriptable.Name}.");
        }

        ConditionalEffect conditionalEffect = null;
        switch (cardScriptable.Condition)
        {
            case ConditionType.Constant:
                conditionalEffect = new ConstantEffect(card, effect);
                break;
            case ConditionType.Position:
                conditionalEffect = new PositionEffect(card, effect, cardScriptable.Position);
                break;
            case ConditionType.NextTo:
                conditionalEffect = new NextToEffect(card, effect, cardScriptable.NextTo);
                break;
            default:
                throw new System.NotSupportedException($"Condition type '{cardScriptable.Condition}' is not supported or implemented yet.");
        }
        return conditionalEffect;
    }
}