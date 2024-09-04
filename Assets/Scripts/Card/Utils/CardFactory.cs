using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiates a Card gameobject based on JSON or Scriptable Object data.
/// </summary>
public static class CardFactory
{
    /// <summary>
    /// Creates a card gameobject from the given prefab and data.
    /// </summary>
    /// <param name="cardPrefab">The prefab to instantiate.</param>
    /// <param name="data">The data to initialize the card with.</param>
    /// <returns>The instantiated card gameobject.</returns>
    public static GameObject CreateCard(GameObject cardPrefab, CardData data)
    {
        if (cardPrefab == null)
        {
            throw new ArgumentNullException(nameof(cardPrefab), "Card prefab cannot be null.");
        }

        GameObject cardObject = GameObject.Instantiate(cardPrefab);
        Card card = cardObject.GetComponent<Card>();

        if (card == null)
        {
            throw new InvalidOperationException("The instantiated object does not have a Card component.");
        }

        CreateCard(card, data);

        return cardObject;
    }


    /// <summary>
    /// Initializes a pre-instantiated card from the given data.
    /// </summary>
    /// <param name="card">The card object to initialize.</param>
    /// <param name="data">The data to initialize the card with.</param>
    public static void CreateCard(Card card, CardData data)
    {
        // Create a dictionary to map triggers to conditions
        Dictionary<CardEffectTriggerType, List<CardCondition>> conditions = new();

        foreach (var conditionData in data.ConditionalEffects)
        {
            CardCondition condition = CreateCondition(conditionData);

            // Add the condition to the corresponding trigger in the dictionary
            if (!conditions.ContainsKey(conditionData.TriggerType))
            {
                conditions[conditionData.TriggerType] = new List<CardCondition>();
            }
            conditions[conditionData.TriggerType].Add(condition);
        }

        // Initialize the card based on its type
        if (card is BuildingCard buildingCard && data is BuildingCardData buildingCardData)
        {
            buildingCard.Initialize(buildingCardData.CardID, buildingCardData.CardName, conditions, buildingCardData.ValidTargets, buildingCardData.Traits);
        }
        else
        {
            throw new InvalidCastException($"Expected BuildingCardData, but got {data.GetType().Name}");
        }
    }

    /// <summary>
    /// Creates a card condition based on the provided data.
    /// </summary>
    /// <param name="conditionData">The data defining the condition.</param>
    /// <returns>The created CardCondition object.</returns>
    public static CardCondition CreateCondition(CardConditionData conditionData)
    {
        CardCondition condition;

        switch (conditionData.ConditionType)
        {
            case CardConditionType.Always:
                condition = new AlwaysCondition();
                break;
            case CardConditionType.Countdown:
                condition = new CountdownCondition(conditionData.ConditionValue);
                break;
            case CardConditionType.Cluster:
                condition = new ClusterCondition(conditionData.ConditionValue);
                break;
            default:
                throw new NotSupportedException($"Condition type '{conditionData.ConditionType}' is not supported.");
        }

        // Add effects to the condition
        foreach (CardEffectData effectData in conditionData.Effects)
        {
            condition.AddEffect(CreateEffect(effectData));
        }

        return condition;
    }

    /// <summary>
    /// Creates a card effect based on the provided data.
    /// </summary>
    /// <param name="effectData">The data defining the effect.</param>
    /// <returns>The created CardEffect object.</returns>
    public static CardEffect CreateEffect(CardEffectData effectData)
    {
        // Split the effect keyword into tokens to determine the effect type and parameters
        List<string> tokens = new(effectData.Keyword.Split(' '));

        // Ensure there is at least one token to process
        if (tokens.Count < 1)
        {
            throw new ArgumentException("Effect data keyword is incomplete.");
        }

        string effectType = tokens[0];

        // Handle different cases based on the effect type
        return effectType switch
        {
            "Produce" => HandleResourceEffect(tokens, effectData),
            "Consume" => HandleResourceEffect(tokens, effectData),
            _ => throw new NotSupportedException($"Effect keyword '{effectType}' is not supported or implemented yet.")
        };
    }

    // Helper method to handle resource-based effects (Produce, Consume, etc.)
    private static CardEffect HandleResourceEffect(List<string> tokens, CardEffectData effectData)
    {
        if (tokens.Count < 2)
        {
            throw new ArgumentException("Resource-based effect requires a resource type.");
        }

        string resourceTypeString = tokens[1];

        if (!Enum.TryParse(resourceTypeString, out ResourceType resourceType))
        {
            throw new InvalidCastException($"Failed to parse ResourceType from '{resourceTypeString}'");
        }

        return tokens[0] switch
        {
            "Produce" => new ProduceCardEffect(effectData.Values[0], resourceType),
            "Consume" => new ConsumeCardEffect(effectData.Values[0], resourceType),
            _ => throw new NotSupportedException($"Resource-based effect type '{tokens[0]}' is not supported or implemented yet.")
        };
    }
}

