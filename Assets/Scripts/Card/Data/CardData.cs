using System.Collections.Generic;

/// <summary>
/// A serializable data class that mirrors the structure of <see cref="CardScriptable"/>.
/// This class is used to store card data in a format suitable for serialization
/// and deserialization, making it easy to save and load card data as JSON or other formats.
/// </summary>
[System.Serializable]
public abstract class CardData
{
    public int CardID;  // Unique identifier for the card
    public string CardName;  // Name of the card
    public List<CardConditionData> ConditionalEffects = new();  // Conditions and effects associated with the card
    public List<string> ValidTargets = new();  // List of valid targets for the card

    // Default constructor for CardData
    protected CardData() { }

    // Constructor for initializing CardData with specific values
    protected CardData(int id, string cardName, List<CardConditionData> conditionalEffects, List<string> validTargets)
    {
        CardID = id;
        CardName = cardName;
        ConditionalEffects = conditionalEffects ?? new();  // Initialize with an empty list if null
        ValidTargets = validTargets ?? new();  // Initialize with an empty list if null
    }
}

/// <summary>
/// A data class specifically for building-type cards, extending <see cref="CardData"/>.
/// Includes additional fields relevant to buildings, such as traits.
/// </summary>
public class BuildingCardData : CardData
{
    public List<string> Traits = new();  // List of traits associated with the building card

    // Default constructor
    public BuildingCardData() { }

    // Constructor for initializing BuildingCardData with specific values
    public BuildingCardData(int id, string cardName, List<CardConditionData> conditionalEffects, List<string> validTargets, List<string> traits)
        : base(id, cardName, conditionalEffects, validTargets)
    {
        Traits = traits ?? new List<string>();  // Initialize with an empty list if null
    }
}

/// <summary>
/// A data class specifically for spell-type cards, extending <see cref="CardData"/>.
/// Includes additional fields relevant to spells, such as target range.
/// </summary>
public class SpellCardData : CardData
{
    public int TargetRange = 0;  // Range of the spell's target, defaulting to 0 (no target required)

    // Default constructor for SpellCardData
    public SpellCardData() { }

    // Constructor for initializing SpellCardData with specific values
    public SpellCardData(int id, string cardName, List<CardConditionData> conditionalEffects, List<string> validTargets, int targetRange)
        : base(id, cardName, conditionalEffects, validTargets)
    {
        TargetRange = targetRange;
    }
}