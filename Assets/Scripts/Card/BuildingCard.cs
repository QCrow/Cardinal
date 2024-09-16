using System.Collections.Generic;

public class BuildingCard : Card
{
    public List<string> Traits = new();
    // public Dictionary<ModifierType, Modifier> Modifiers = new();  // TODO: Take modifier from parent to this child
    public void Initialize(int cardID, string cardName, Dictionary<CardEffectTriggerType, List<CardCondition>> conditionalEffects, List<string> validTargets, List<string> traits)
    {
        CardVisual.Initialize(cardID, cardName);
        CardID = cardID;
        CardName = cardName;
        ConditionalEffects = conditionalEffects ?? new();
        ValidTargets = validTargets;
        Traits = traits;
        Modifiers = new();
    }
}