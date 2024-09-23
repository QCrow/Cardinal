// using System;

// /// <summary>
// /// A serializable data class that mirrors the structure of <see cref="CardScriptable"/>.
// /// This class is used to store card data in a format suitable for serialization
// /// and deserialization, making it easy to save and load card data as JSON or other formats.
// /// </summary>
// [Serializable]
// public class CardData
// {
//     public int ID;
//     public string Name;
//     public RarityType Rarity;
//     public ClassType Class;
//     public TraitType Type;

//     public int BaseAttack;

//     // New fields for effect handling
//     public bool HasEffect;
//     public TriggerType Trigger;
//     public ConditionType Condition;
//     public PositionType Position;  // Only relevant if Condition == Position
//     public EffectKeyword Keyword;
//     public ModifierType Modifier;  // Only relevant if Keyword == Apply
//     public int Value;
//     public bool IsTargeted;
//     public TargetRangeType TargetRange; // Only relevant if IsTargeted and certain keywords
//     public TraitType TargetTrait;  // Only relevant if IsTargeted

//     public CardData() { }

//     public CardData(CardScriptable card)
//     {
//         ID = card.ID;
//         Name = card.Name;
//         Rarity = card.Rarity;
//         Class = card.Class;
//         Type = card.Type;
//         BaseAttack = card.BaseAttack;

//         // Effect related fields
//         HasEffect = card.HasEffect;
//         Condition = card.Condition;
//         Position = card.Position;
//         Keyword = card.Keyword;
//         Modifier = card.Modifier;
//         Value = card.Value;
//         IsTargeted = card.IsTargeted;
//         TargetRange = card.TargetRange;
//         TargetTrait = card.TargetTrait;
//     }
// }