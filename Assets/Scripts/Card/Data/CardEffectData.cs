using System.Collections.Generic;

/// <summary>
/// Represents the data needed to define a game effect, including its keyword and associated values.
/// This class is used for serializing and deserializing effect data and is passed to the effect factory
/// to construct specific game effects based on the provided information.
/// </summary>
[System.Serializable]
public class CardEffectData
{
    public string Keyword;  // The keyword identifying the effect (e.g., "Produce", "Consume")
    public List<int> Values = new();  // A list of values associated with the effect (e.g., amount of resources)

    public CardEffectData(string keyword, List<int> values)
    {
        Keyword = keyword;
        Values = values;
    }
}