using System.Collections.Generic;

public class BuildingCard : Card
{
    public List<string> Traits;
    public Dictionary<CardModifier, int> Modifiers;

    public void Initialize(int cardID, string cardName, Dictionary<CardEffectTriggerType, List<CardCondition>> conditionsWithEffects, List<string> validTargets, List<string> traits)
    {
        CardID = cardID;
        CardName = cardName;
        ConditionsWithEffects = conditionsWithEffects;
        ValidTargets = validTargets;
        Traits = traits;

        _cardVisual.Initialize(cardID, cardName);
    }
}