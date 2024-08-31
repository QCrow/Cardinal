using System.Collections.Generic;
using UnityEngine.Playables;

/// <summary>
/// A serializable data class that mirrors the structure of <see cref="CardScriptable"/>.
/// This class is used to store card data in a format suitable for serialization
/// and deserialization, making it easy to save and load card data as JSON or other formats.
/// </summary>
[System.Serializable]
public abstract class CardData
{
    public int ID;
    public string CardName;
    public List<CardConditionData> ConditionsWithEffects = new();
    public List<string> ValidTargets = new();

    protected CardData() { }

    // Constructor for CardData
    protected CardData(int id, string cardName, List<CardConditionData> conditionsWithEffects, List<string> validTargets)
    {
        ID = id;
        CardName = cardName;
        ConditionsWithEffects = conditionsWithEffects;  // Initialize with an empty list if null
        ValidTargets = validTargets;  // Initialize with an empty list if null
    }
}


public class BuildingCardData : CardData
{
    public List<string> BuildingTraits = new();

    public BuildingCardData() { }

    // Constructor for BuildingCardData
    public BuildingCardData(int id, string cardName, List<CardConditionData> conditionsWithEffects, List<string> validTargets, List<string> buildingTraits)
        : base(id, cardName, conditionsWithEffects, validTargets)
    {
        BuildingTraits = buildingTraits;  // Initialize with an empty list if null
    }
}

public class SpellCardData : CardData
{
    public int TargetRange = 1;

    public SpellCardData() { }

    // Constructor for SpellCardData
    public SpellCardData(int id, string cardName, List<CardConditionData> conditionsWithEffects, List<string> validTargets, int targetRange)
        : base(id, cardName, conditionsWithEffects, validTargets)
    {
        TargetRange = targetRange;
    }
}