// using UnityEngine;
// using Sirenix.OdinInspector;
// using System.IO;
// using UnityEditor;
// using System.Collections.Generic;
// using Newtonsoft.Json;

// /// <summary>
// /// Utility class for serializing and deserializing CardScriptable objects
// /// to and from JSON files.
// /// </summary>
// public class CardSerializer : SerializedMonoBehaviour
// {
//     [SerializeField, FolderPath]
//     [Tooltip("Path to the folder where Scriptable objects are stored")]
//     private string _scriptablePath = "Assets/Resources";  // Path for ScriptableObject storage

//     [SerializeField, FolderPath]
//     [Tooltip("Path to the folder where Serialized JSON files will be saved")]
//     private string _serializedPath = "Assets/Serialized";  // Path for JSON storage

//     private const string _cardJsonName = "Cards.json";  // Name of the JSON file

//     #region Cards
//     /// <summary>
//     /// Serializes all CardScriptable objects found in the specified path to a single JSON file.
//     /// </summary>
//     [Button(ButtonSizes.Large), GUIColor(0.6f, 0.8f, 1.0f)]
//     [VerticalGroup("Save Group")]
//     private void SerializeAllCardsToJson()
//     {
//         if (EditorUtility.DisplayDialog("Confirm Save", "Are you sure you want to save all CardScriptables to JSON?", "Yes", "No"))
//         {
//             string cardScriptablePath = Path.Combine(_scriptablePath, "Cards");
//             string cardSerializedPath = Path.Combine(_serializedPath, "Cards");

//             try
//             {
//                 Directory.CreateDirectory(cardSerializedPath);

//                 string[] guids = AssetDatabase.FindAssets("t:CardScriptable", new[] { cardScriptablePath });
//                 List<CardData> cardList = new List<CardData>();

//                 foreach (string guid in guids)
//                 {
//                     string path = AssetDatabase.GUIDToAssetPath(guid);
//                     CardScriptable card = AssetDatabase.LoadAssetAtPath<CardScriptable>(path);

//                     if (card != null)
//                     {
//                         // Convert CardScriptable to CardData
//                         CardData data = new CardData(card);
//                         cardList.Add(data);
//                     }
//                     else
//                     {
//                         Debug.LogWarning($"Failed to load CardScriptable at path: {path}");
//                     }
//                 }

//                 if (cardList.Count > 0)
//                 {
//                     // Use the custom JsonSerializerSettings
//                     string json = JsonConvert.SerializeObject(cardList, JsonSettingsProvider.CardJsonSerializerSettings);
//                     string filePath = Path.Combine(cardSerializedPath, _cardJsonName);
//                     File.WriteAllText(filePath, json);
//                     AssetDatabase.Refresh();

//                     Debug.Log($"Serialized {cardList.Count} CardScriptable objects to JSON at {filePath}");
//                 }
//                 else
//                 {
//                     Debug.LogWarning("No CardScriptable objects found to serialize.");
//                 }
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Failed to serialize CardScriptable objects: {ex.Message}\n{ex.StackTrace}");
//             }
//         }
//     }


//     /// <summary>
//     /// Deserializes all CardScriptable objects from the JSON file and recreates them.
//     /// </summary>
//     [Button(ButtonSizes.Large), GUIColor(1.0f, 0.6f, 0.6f)]
//     [VerticalGroup("Load Group")]
//     private void DeserializeAllCardsFromJson()
//     {
//         if (EditorUtility.DisplayDialog("Confirm Load", "Are you sure you want to load from JSON and overwrite existing ScriptableObjects?", "Yes", "No"))
//         {
//             string jsonPath = Path.Combine(_serializedPath, "Cards", _cardJsonName);
//             string json = File.ReadAllText(jsonPath);

//             // Deserialize using the custom JsonSerializerSettings
//             List<CardData> cardDatas = JsonConvert.DeserializeObject<List<CardData>>(json, JsonSettingsProvider.CardJsonSerializerSettings);

//             // Delete existing ScriptableObjects in the directory
//             string directory = Path.Combine(_scriptablePath, "Cards");
//             Directory.CreateDirectory(directory);
//             string[] files = Directory.GetFiles(directory);
//             foreach (string file in files)
//             {
//                 try
//                 {
//                     File.Delete(file);
//                 }
//                 catch (System.Exception ex)
//                 {
//                     Debug.LogError($"Failed to delete file {file}: {ex.Message}");
//                 }
//             }

//             // Recreate ScriptableObjects based on deserialized data
//             foreach (var cardData in cardDatas)
//             {
//                 CardScriptable card = ScriptableObject.CreateInstance<CardScriptable>();

//                 card.ID = cardData.ID;
//                 card.Name = cardData.Name;
//                 card.Rarity = cardData.Rarity;
//                 card.Class = cardData.Class;
//                 card.Type = cardData.Type;
//                 card.BaseAttack = cardData.BaseAttack;

//                 // Restore effect fields
//                 card.HasEffect = cardData.HasEffect;
//                 card.Condition = cardData.Condition;
//                 card.Position = cardData.Position;
//                 card.Keyword = cardData.Keyword;
//                 card.Value = cardData.Value;
//                 card.IsTargeted = cardData.IsTargeted;
//                 card.TargetRange = cardData.TargetRange;
//                 card.TargetTrait = cardData.TargetTrait;


//                 SaveCardAsset(card);
//             }

//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//         }
//     }


//     /// <summary>
//     /// Saves the provided CardScriptable object as an asset in the predefined path.
//     /// </summary>
//     /// <param name="card">The card scriptable object to save.</param>
//     private void SaveCardAsset(CardScriptable card)
//     {
//         string directory = Path.Combine(_scriptablePath, "Cards");
//         Directory.CreateDirectory(directory);

//         string assetPath = Path.Combine(directory, $"Card_{card.ID}.asset");
//         // Set object name and save as asset
//         card.name = $"Card_{card.ID}";

//         // Check if an existing asset needs to be overwritten
//         CardScriptable existingCard = AssetDatabase.LoadAssetAtPath<CardScriptable>(assetPath);
//         if (existingCard != null)
//         {
//             EditorUtility.CopySerialized(card, existingCard);
//         }
//         else
//         {
//             AssetDatabase.CreateAsset(card, assetPath);
//         }

//         Debug.Log($"Saved card {card.ID}: {card.Name} to {assetPath}");
//     }
//     #endregion
// }