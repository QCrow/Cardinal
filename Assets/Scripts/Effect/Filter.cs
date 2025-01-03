using System;
using Sirenix.OdinInspector;

public enum FilterType
{
    None,
    ID,
    Name,
    Rarity,
    NameContains,
}


[Serializable]
public class Filter
{
    public FilterType Type;
    [ShowIf("Type", FilterType.ID)]
    public int ID;
    [ShowIf("Type", FilterType.Name)]
    public string Name;
    [ShowIf("Type", FilterType.Rarity)]
    public CardRarityType Rarity;
    [ShowIf("Type", FilterType.NameContains)]
    public string NameContains;

    public bool IsMatch(Card card)
    {
        return Type switch
        {
            FilterType.ID => card.ID == ID,
            FilterType.Name => card.CardName == Name,
            FilterType.Rarity => card.Rarity == Rarity,
            FilterType.NameContains => card.CardName.Contains(NameContains),
            _ => false,
        };
    }
}