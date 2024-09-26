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

public enum SelectorCheckType
{
    Exists,
    Minimum,
    Count
}

[Serializable]
public class Selector
{
    public SelectorType Type;
    public SelectorCheckType Check;
    [ShowIf("Check", SelectorCheckType.Minimum)]
    [Tooltip("The minimum number of neighbors that must match the selector for the effect to activate.")]
    public int Minimum;
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