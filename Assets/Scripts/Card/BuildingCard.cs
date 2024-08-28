using System.Collections.Generic;


public class BuildingCard : Card
{
    private ColorType _colorType;
    public ColorType ColorType { get => _colorType; set => _colorType = value; }

    public Dictionary<ColorType, int> BUildingOutput { get; set; }

    public List<BuildingCardEffect> OnPlayEffects;
    public List<BuildingCardEffect> OnTurnEndEffects;

    //TODO: Add initializers for additional properties
    public void Initialize(int cardID, string cardName, string cardDescription, ColorType colorType)
    {
        CardID = cardID;
        _colorType = colorType;

        _cardVisual.Initialize(cardID, cardName, cardDescription, colorType);
    }
}