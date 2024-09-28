using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum FilterType
{
    None,
    ID,
    Name,
    Trait,
    Rarity
}


[Serializable]
public class Filter
{
    public FilterType Type;
    [ShowIf("Type", FilterType.ID)]
    public int ID;
    [ShowIf("Type", FilterType.Name)]
    public string Name;
    [ShowIf("Type", FilterType.Trait)]
    public TraitType Trait;
    [ShowIf("Type", FilterType.Rarity)]
    public RarityType Rarity;

    public bool IsMatch(Card card)
    {
        return Type switch
        {
            FilterType.ID => card.ID == ID,
            FilterType.Name => card.Name == Name,
            FilterType.Trait => Trait switch
            {
                TraitType.All => true,
                TraitType.None => false,
                _ => card.Traits.Contains(Trait),
            },
            FilterType.Rarity => card.Rarity == Rarity,
            _ => false,
        };
    }
}