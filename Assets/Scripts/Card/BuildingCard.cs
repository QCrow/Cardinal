using System.Collections.Generic;

public class BuildingCard : Card
{
    public List<string> Traits = new();
    public Dictionary<CardModifier, int> Modifiers = new();

    public void Initialize(int cardID, string cardName, Dictionary<CardEffectTriggerType, List<CardCondition>> conditionsWithEffects, List<string> validTargets, List<string> traits)
    {
        _cardVisual.Initialize(cardID, cardName);
        CardID = cardID;
        CardName = cardName;
        ConditionsWithEffects = conditionsWithEffects;
        ValidTargets = validTargets;
        Traits = traits;
        Modifiers = new();
    }
}