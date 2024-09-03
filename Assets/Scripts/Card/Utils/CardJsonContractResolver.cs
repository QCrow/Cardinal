using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Custom contract resolver for serializing and deserializing JSON objects while excluding certain properties.
/// </summary>
public class CardJsonContractResolver : DefaultContractResolver
{
    /// <summary>
    /// Creates a JSON property based on the member information provided, with custom logic to exclude certain properties.
    /// </summary>
    /// <param name="member">The member information.</param>
    /// <param name="memberSerialization">The member serialization information.</param>
    /// <returns>A JsonProperty representing the member.</returns>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // Exclude 'name' and 'hideFlags' properties from serialization
        if (property.PropertyName == "name" || property.PropertyName == "hideFlags")
        {
            property.ShouldSerialize = instance => false;
        }
        return property;
    }
}
