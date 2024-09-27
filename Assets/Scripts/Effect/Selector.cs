using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum SelectorType
{
    None,
    ID,
    Name,
    Trait,
    Rarity
}


[Serializable]
public class Selector
{
    public SelectorType Type;
    [ShowIf("Type", SelectorType.ID)]
    public int ID;
    [ShowIf("Type", SelectorType.Name)]
    public string Name;
    [ShowIf("Type", SelectorType.Trait)]
    public TraitType Trait;
    [ShowIf("Type", SelectorType.Rarity)]
    public RarityType Rarity;

    public bool IsMatch(Card card)
    {
        return Type switch
        {
            SelectorType.ID => card.ID == ID,
            SelectorType.Name => card.Name == Name,
            SelectorType.Trait => Trait switch
            {
                TraitType.All => true,
                TraitType.None => false,
                _ => card.Trait == Trait,
            },
            SelectorType.Rarity => card.Rarity == Rarity,
            _ => false,
        };
    }
}