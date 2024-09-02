using System;
using System.Collections.Generic;


/// <summary>
/// Instantiates a Card gameobject based on JSON or Scriptable Object data
/// </summary>
public static class CardFactory
{
    public static void CreateCard(Card card, CardData data)
    {
        Dictionary<CardEffectTriggerType, List<CardCondition>> conditions = new();
        foreach (var conditionData in data.ConditionsWithEffects)
        {
            CardCondition condition = CreateCondition(conditionData);

            // Try to append to a list of existing Trigger, otherwise create a new key value pair
            if (!conditions.ContainsKey(conditionData.Trigger))
            {
                conditions[conditionData.Trigger] = new List<CardCondition>();
            }
            conditions[conditionData.Trigger].Add(condition);
        }

        if (card is BuildingCard buildingCard)
        {
            if (data is BuildingCardData buildingCardData)
            {
                buildingCard.Initialize(buildingCardData.CardID, buildingCardData.CardName, conditions, buildingCardData.ValidTargets, buildingCardData.Traits);
            }
            else
            {
                throw new InvalidCastException($"Expected BuildingCardData, but got {data.GetType().Name}");
            }
        }
    }

    public static CardCondition CreateCondition(CardConditionData conditionData)
    {
        CardCondition condition;
        switch (conditionData.Type)
        {
            case "Always":
                condition = new AlwaysCondition();
                break;
            case "Countdown":
                condition = new CountdownCondition(conditionData.Value);
                break;
            default:
                condition = new AlwaysCondition(); // TODO: Implement error checking
                break;
        }

        foreach (CardEffectData effectData in conditionData.Effects)
        {
            if (condition.Effects == null) condition.Effects = new();
            condition.Effects.Add(CreateEffect(effectData));
        }

        return condition;
    }

    public static CardEffect CreateEffect(CardEffectData effectData)
    {
        List<string> tokens = new List<string>(effectData.Keyword.Split(' '));
        switch (tokens[0])
        {
            case "Produce":
                return new ProduceCardEffect(effectData.Values[0], tokens[1]);
            case "Consume":
                return new ConsumeCardEffect(effectData.Values[0], tokens[1]);
            default:
                throw new NotSupportedException($"Effect keyword '{tokens[0]}' is not supported or implemented yet.");
        }
    }
}