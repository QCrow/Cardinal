using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System;
using Newtonsoft.Json.Linq;

public class DataUtil : MonoBehaviour
{
    [SerializeField, FolderPath]
    [Tooltip("Path to the foler where Scriptable objects are stored")]
    private string _scriptablePath = "Assets/Resources";

    [SerializeField, FolderPath]
    [Tooltip("Path to the foler where Serialized JSON files will be saved")]
    private string _serializedPath = "Assets/Serialized";

    private const string _cardJsonName = "Cards.json";

    private class CustomContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyName == "name" || property.PropertyName == "hideFlags")
            {
                property.ShouldSerialize = instance => false;
            }
            return property;
        }
    }

    readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        ContractResolver = new CustomContractResolver(),
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto
    };

    #region Cards
    /// <summary>
    /// Serializes all <see cref="CardScriptable"/> objects found in the specified path to a single JSON file.
    /// </summary>
    [Button("Save to JSON")]
    private void SerializeAllCardsToJson()
    {
        // Define the paths for locating CardScriptable objects and saving the serialized JSON
        string cardScriptablePath = Path.Combine(_scriptablePath, "Cards");
        string cardSerializedPath = Path.Combine(_serializedPath, "Cards");

        try
        {
            // Ensure the serialized directory exists, create it if not
            Directory.CreateDirectory(cardSerializedPath);

            // Find all CardScriptable objects in the specified path
            string[] guids = AssetDatabase.FindAssets("t:CardScriptable", new[] { cardScriptablePath });
            List<CardData> cardList = new List<CardData>();

            // Load each CardScriptable object and add it to the list
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                CardScriptable card = AssetDatabase.LoadAssetAtPath<CardScriptable>(path);

                if (card == null)
                {
                    Debug.LogWarning($"Failed to load CardScriptable at path: {path}");
                    continue;
                }

                // Handle each specific type of CardScriptable and map it to the corresponding CardData
                if (card is BuildingCardScriptable buildingCard)
                {
                    BuildingCardData data = new BuildingCardData(
                        buildingCard.ID,
                        buildingCard.CardName,
                        buildingCard.Effects,
                        buildingCard.ValidTargets,
                        buildingCard.BuildingTraits
                    );
                    cardList.Add(data);
                }
                else if (card is SpellCardScriptable spellCard)
                {
                    SpellCardData data = new SpellCardData(
                        spellCard.ID,
                        spellCard.CardName,
                        spellCard.Effects,
                        spellCard.ValidTargets,
                        spellCard.TargetRange
                    );
                    cardList.Add(data);
                }
            }

            // Check if any CardScriptable objects were found
            if (cardList.Count == 0)
            {
                Debug.LogWarning("No CardScriptable objects found to serialize.");
                return;
            }



            // Serialize the list of CardScriptable objects to JSON
            string json = JsonConvert.SerializeObject(cardList, _jsonSettings);

            // Define the file path for saving the serialized JSON
            string filePath = Path.Combine(cardSerializedPath, _cardJsonName);

            // Write the JSON to a file
            File.WriteAllText(filePath, json);

            // Refresh the AssetDatabase to ensure the new file is recognized by Unity
            AssetDatabase.Refresh();

            Debug.Log($"Serialized {cardList.Count} CardScriptable objects to JSON file at {filePath}");
        }
        catch (System.Exception ex)
        {
            // Log detailed error information
            Debug.LogError($"Failed to serialize CardScriptable objects: {ex.Message}\n{ex.StackTrace}");
        }
    }

    [Button("Load from JSON")]
    private void DeserializeAllCardsFromJson()
    {
        string jsonPath = Path.Combine(_serializedPath, "Cards", _cardJsonName);
        string json = File.ReadAllText(jsonPath);

        // Using the helper data class to avoid directly constructing scriptable objects
        List<CardData> cardDatas = JsonConvert.DeserializeObject<List<CardData>>(json, _jsonSettings);

        // Delete all existing scriptable objects in the scriptable directory
        string directory = Path.Combine(_scriptablePath, "Cards");
        Directory.CreateDirectory(directory);
        string[] files = Directory.GetFiles(directory);
        foreach (string file in files)
        {
            try
            {
                File.Delete(file);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to delete file {file}: {ex.Message}");
            }
        }

        foreach (var cardData in cardDatas)
        {
            if (cardData is BuildingCardData buildingCardData)
            {
                BuildingCardScriptable card = ScriptableObject.CreateInstance<BuildingCardScriptable>();
                card.ID = buildingCardData.ID;
                card.CardName = buildingCardData.CardName;
                card.Effects = buildingCardData.Effects;
                card.BuildingTraits = buildingCardData.BuildingTraits;
                card.name = $"Card_{buildingCardData.ID}";
                SaveCardAsset(card);
            }
            else if (cardData is SpellCardData spellCardData)
            {
                SpellCardScriptable card = ScriptableObject.CreateInstance<SpellCardScriptable>();
                card.ID = spellCardData.ID;
                card.CardName = spellCardData.CardName;
                card.Effects = spellCardData.Effects;
                card.TargetRange = spellCardData.TargetRange;
                card.name = $"Card_{spellCardData.ID}";
                SaveCardAsset(card);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SaveCardAsset(CardScriptable card)
    {
        // Ensure the scriptable path exists
        string directory = Path.Combine(_scriptablePath, "Cards");
        Directory.CreateDirectory(directory);

        string assetPath = Path.Combine(directory, $"Card_{card.ID}.asset");

        CardScriptable existingCard = AssetDatabase.LoadAssetAtPath<CardScriptable>(assetPath);
        if (existingCard != null)
        {
            EditorUtility.CopySerialized(card, existingCard);
        }
        else
        {
            AssetDatabase.CreateAsset(card, assetPath);
        }

        Debug.Log($"Saved card {card.ID}: {card.CardName} to {assetPath}");
    }
    #endregion
}