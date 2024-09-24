using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum SelectorType
{
    None,
    ID,
    Name,
    Trait,
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


    public bool IsMatch(Card card)
    {
        switch (Type)
        {
            case SelectorType.ID:
                return card.ID == ID;
            case SelectorType.Name:
                return card.Name == Name;
            case SelectorType.Trait:
                return card.Trait == Trait;
            default:
                return false;
        }
    }
}