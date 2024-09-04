using System.Collections.Generic;

/// <summary>
/// Represents the data required to define a condition for triggering card effects.
/// This class is used to serialize and deserialize condition data, which is then
/// used to determine when certain effects should be applied in the game.
/// </summary>
[System.Serializable]
public class CardConditionData
{
    public CardConditionType ConditionType;  // The type of condition (e.g., "Always", "Countdown")
    public CardEffectTriggerType TriggerType;  // The event that triggers this condition (e.g., "OnPlay", "OnEndTurn")
    public int ConditionValue;  // An optional value associated with the condition (e.g., countdown value)

    public List<CardEffectData> Effects = new();  // The list of effects that are triggered when this condition is met

    // Default constructor
    public CardConditionData() { }
}
