using System.Collections.Generic;

public class BuildingCard : Card
{
    public List<string> Traits;

    //TODO: Add initializers for additional properties
    public void Initialize(int cardID, string cardName, Dictionary<string, List<CardEffect>> effects, List<string> validTargets, List<string> traits)
    {
        CardID = cardID;
        CardName = cardName;
        Effects = effects;
        ValidTargets = validTargets;
        Traits = traits;

        _cardVisual.Initialize(cardID, cardName);
    }
}