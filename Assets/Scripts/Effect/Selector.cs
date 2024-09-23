using System;
using Sirenix.OdinInspector;

public enum SelectorType
{
    None,
    ID,
    Name,
    Trait,
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
}