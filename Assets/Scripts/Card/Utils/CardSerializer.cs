using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Utility class for serializing and deserializing CardScriptable objects
/// to and from JSON files.
/// </summary>
public class CardSerializer : SerializedMonoBehaviour
{
    [SerializeField, FolderPath]
    [Tooltip("Path to the folder where Scriptable objects are stored")]
    private string _scriptablePath = "Assets/Resources";  // Path for ScriptableObject storage

    [SerializeField, FolderPath]
    [Tooltip("Path to the folder where Serialized JSON files will be saved")]
    private string _serializedPath = "Assets/Serialized";  // Path for JSON storage

    private const string _cardJsonName = "Cards.json";  // Name of the JSON file

    #region Cards
    /// <summary>
    /// Serializes all CardScriptable objects found in the specified path to a single JSON file.
    /// </summary>
    [Button(ButtonSizes.Large), GUIColor(0.6f, 0.8f, 1.0f)]
    [VerticalGroup("Save Group")]
    private void SerializeAllCardsToJson()
    {
        // Confirm with the user before proceeding with serialization
        if (EditorUtility.DisplayDialog("Confirm Save", "Are you sure you want to save all CardScriptables to JSON? This will overwrite the existing JSON file.", "Yes", "No"))
        {
            string cardScriptablePath = Path.Combine(_scriptablePath, "Cards");
            string cardSerializedPath = Path.Combine(_serializedPath, "Cards");

            try
            {
                // Ensure the directory for serialized JSON exists
                Directory.CreateDirectory(cardSerializedPath);

                // Find and load all CardScriptable objects
                string[] guids = AssetDatabase.FindAssets("t:CardScriptable", new[] { cardScriptablePath });
                List<CardData> cardList = new List<CardData>();

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    CardScriptable card = AssetDatabase.LoadAssetAtPath<CardScriptable>(path);

                    if (card == null)
                    {
                        Debug.LogWarning($"Failed to load CardScriptable at path: {path}");
                        continue;
                    }

                    // Convert and add each card to the list based on its type
                    if (card is BuildingCardScriptable buildingCard)
                    {
                        BuildingCardData data = new BuildingCardData(
                            buildingCard.ID,
                            buildingCard.CardName,
                            buildingCard.ConditionalEffects,
                            buildingCard.ValidTargets,
                            buildingCard.Traits
                        );
                        cardList.Add(data);
                    }
                    else if (card is SpellCardScriptable spellCard)
                    {
                        SpellCardData data = new SpellCardData(
                            spellCard.ID,
                            spellCard.CardName,
                            spellCard.ConditionalEffects,
                            spellCard.ValidTargets,
                            spellCard.TargetRange
                        );
                        cardList.Add(data);
                    }
                }

                if (cardList.Count == 0)
                {
                    Debug.LogWarning("No CardScriptable objects found to serialize.");
                    return;
                }

                // Serialize the list of cards to JSON and save to file
                string json = JsonConvert.SerializeObject(cardList, JsonSettingsProvider.CardJsonSerializerSettings);
                string filePath = Path.Combine(cardSerializedPath, _cardJsonName);
                File.WriteAllText(filePath, json);

                // Refresh the AssetDatabase to recognize the new file
                AssetDatabase.Refresh();

                Debug.Log($"Serialized {cardList.Count} CardScriptable objects to JSON file at {filePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to serialize CardScriptable objects: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    /// <summary>
    /// Deserializes all CardScriptable objects from the JSON file and recreates them.
    /// </summary>
    [Button(ButtonSizes.Large), GUIColor(1.0f, 0.6f, 0.6f)]
    [VerticalGroup("Load Group")]
    private void DeserializeAllCardsFromJson()
    {
        // Confirm with the user before proceeding with deserialization
        if (EditorUtility.DisplayDialog("Confirm Load", "Are you sure you want to load from JSON and overwrite existing ScriptableObjects? This will delete all existing ScriptableObjects and recreate them.", "Yes", "No"))
        {
            string jsonPath = Path.Combine(_serializedPath, "Cards", _cardJsonName);
            string json = File.ReadAllText(jsonPath);

            // Deserialize the JSON data into a list of CardData objects
            List<CardData> cardDatas = JsonConvert.DeserializeObject<List<CardData>>(json, JsonSettingsProvider.CardJsonSerializerSettings);

            // Delete existing ScriptableObjects in the directory
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

            // Recreate ScriptableObjects based on the deserialized data
            foreach (var cardData in cardDatas)
            {
                if (cardData is BuildingCardData buildingCardData)
                {
                    BuildingCardScriptable buildingCard = ScriptableObject.CreateInstance<BuildingCardScriptable>();
                    buildingCard.ID = buildingCardData.CardID;
                    buildingCard.CardName = buildingCardData.CardName;
                    buildingCard.ConditionalEffects = buildingCardData.ConditionalEffects;
                    buildingCard.Traits = buildingCardData.Traits;
                    buildingCard.name = $"Card_{buildingCardData.CardID}";
                    SaveCardAsset(buildingCard);
                }
                else if (cardData is SpellCardData spellCardData)
                {
                    SpellCardScriptable spellCard = ScriptableObject.CreateInstance<SpellCardScriptable>();
                    spellCard.ID = spellCardData.CardID;
                    spellCard.CardName = spellCardData.CardName;
                    spellCard.ConditionalEffects = spellCardData.ConditionalEffects;
                    spellCard.TargetRange = spellCardData.TargetRange;
                    spellCard.name = $"Card_{spellCardData.CardID}";
                    SaveCardAsset(spellCard);
                }
            }

            // Save the created assets and refresh the AssetDatabase
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// Saves the provided CardScriptable object as an asset in the predefined path.
    /// </summary>
    /// <param name="card">The card scriptable object to save.</param>
    private void SaveCardAsset(CardScriptable card)
    {
        string directory = Path.Combine(_scriptablePath, "Cards");
        Directory.CreateDirectory(directory);

        string assetPath = Path.Combine(directory, $"Card_{card.ID}.asset");

        // Check if an existing asset needs to be overwritten
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