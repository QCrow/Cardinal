
using System.Collections.Generic;

/// <summary>
/// Represents the data required to define an effect in the game.
/// This class is used for serializing and deserializing effect data, 
/// and is passed as an argument to the effect factory for constructing 
/// specific game effects based on the provided information.
/// </summary>
[System.Serializable]
public class EffectData
{
    public string Keyword;
    public string Trigger;
    public List<int> Values = new();
}