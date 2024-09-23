// using Newtonsoft.Json;
// using Newtonsoft.Json.Converters;

// public static class JsonSettingsProvider
// {
//     /// <summary>
//     /// Public static readonly instance of JsonSerializerSettings configured with the custom contract resolver and other settings.
//     /// </summary>
//     public static readonly JsonSerializerSettings CardJsonSerializerSettings = new JsonSerializerSettings
//     {
//         ContractResolver = new CardJsonContractResolver(),
//         Formatting = Formatting.Indented,
//         TypeNameHandling = TypeNameHandling.Auto,
//         Converters = { new StringEnumConverter(), new CardDataConverter() }
//     };
// }