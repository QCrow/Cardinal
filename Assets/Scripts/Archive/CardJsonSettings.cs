// using System;
// using System.Reflection;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
// using Newtonsoft.Json.Serialization;

// /// <summary>
// /// Custom contract resolver for serializing and deserializing JSON objects while excluding certain properties.
// /// </summary>
// public class CardJsonContractResolver : DefaultContractResolver
// {
//     /// <summary>
//     /// Creates a JSON property based on the member information provided, with custom logic to exclude certain properties.
//     /// </summary>
//     /// <param name="member">The member information.</param>
//     /// <param name="memberSerialization">The member serialization information.</param>
//     /// <returns>A JsonProperty representing the member.</returns>
//     protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
//     {
//         JsonProperty property = base.CreateProperty(member, memberSerialization);

//         // Exclude 'name' and 'hideFlags' properties from serialization
//         if (property.PropertyName == "name" || property.PropertyName == "hideFlags")
//         {
//             property.ShouldSerialize = instance => false;
//         }
//         return property;
//     }
// }

// public class CardDataConverter : JsonConverter<CardData>
// {
//     public override CardData ReadJson(JsonReader reader, Type objectType, CardData existingValue, bool hasExistingValue, JsonSerializer serializer)
//     {
//         JObject jsonObject = JObject.Load(reader);
//         CardData cardData = new CardData
//         {
//             ID = jsonObject["ID"].Value<int>(),
//             Name = jsonObject["Name"].Value<string>(),
//             Rarity = jsonObject["Rarity"].ToObject<RarityType>(),
//             Class = jsonObject["Class"].ToObject<ClassType>(),
//             Type = jsonObject["Type"].ToObject<TraitType>(),
//             BaseAttack = jsonObject["BaseAttack"].Value<int>(),
//             HasEffect = jsonObject["HasEffect"].Value<bool>(),
//             Trigger = jsonObject["Trigger"]?.ToObject<TriggerType>() ?? TriggerType.None,
//             Condition = jsonObject["Condition"]?.ToObject<ConditionType>() ?? ConditionType.None,
//             Position = jsonObject["Position"]?.ToObject<PositionType>() ?? PositionType.None,
//             Keyword = jsonObject["Keyword"]?.ToObject<EffectKeyword>() ?? EffectKeyword.None,
//             Value = jsonObject["Value"]?.Value<int>() ?? 0,
//             IsTargeted = jsonObject["IsTargeted"]?.Value<bool>() ?? false,
//             TargetRange = jsonObject["TargetRange"]?.ToObject<TargetRangeType>() ?? TargetRangeType.None,
//             TargetTrait = jsonObject["TargetTrait"]?.ToObject<TraitType>() ?? TraitType.None
//         };

//         return cardData;
//     }

//     public override void WriteJson(JsonWriter writer, CardData value, JsonSerializer serializer)
//     {
//         JObject jsonObject = new JObject
//     {
//         { "ID", value.ID },
//         { "Name", value.Name },
//         { "Rarity", JToken.FromObject(value.Rarity, serializer) },
//         { "Class", JToken.FromObject(value.Class, serializer) },
//         { "Type", JToken.FromObject(value.Type, serializer) },
//         { "BaseAttack", value.BaseAttack },
//         { "HasEffect", value.HasEffect }
//     };

//         if (value.HasEffect)
//         {
//             jsonObject.Add("Trigger", JToken.FromObject(value.Trigger, serializer));
//             jsonObject.Add("Condition", JToken.FromObject(value.Condition, serializer));
//             if (value.Condition == ConditionType.Position)
//             {
//                 jsonObject.Add("Position", JToken.FromObject(value.Position, serializer));
//             }
//             jsonObject.Add("Keyword", JToken.FromObject(value.Keyword, serializer));
//             jsonObject.Add("Value", value.Value);
//             if (value.IsTargeted)
//             {
//                 jsonObject.Add("TargetRange", JToken.FromObject(value.TargetRange, serializer));
//                 jsonObject.Add("TargetTrait", JToken.FromObject(value.TargetTrait, serializer));
//             }
//         }

//         jsonObject.WriteTo(writer);
//     }
// }