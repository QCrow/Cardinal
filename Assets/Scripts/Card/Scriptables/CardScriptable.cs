using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEditor;
using System.IO;

public abstract class CardScriptable : SerializedScriptableObject
{
    [OnValueChanged("ValidateAndUpdateID")]
    [Delayed]
    [Tooltip("Unique identifier for the card. This ID should be unique within the directory.")]
    public int ID;

    [JsonIgnore]
    private int _prevID;

    [Tooltip("Name of the card.")]
    public string CardName;

    [Space(10)]
    [ListDrawerSettings(DefaultExpandedState = true, ShowIndexLabels = true)]
    [Tooltip("Effects associated with the card, grouped by condition.")]
    [ValidateInput("HasConditions", "Conditions With Effects cannot be empty.")]
    public List<CardConditionData> ConditionalEffects = new();

    [Space(10)]
    [ListDrawerSettings(DefaultExpandedState = true, ShowIndexLabels = true)]
    [Tooltip("List of valid targets for the card.")]
    [ValidateInput("HasValidTargets", "Valid Targets cannot be empty.")]
    public List<string> ValidTargets = new();

    private void OnEnable()
    {
        _prevID = ID;
    }

    /// <summary>
    /// Validates that the ID is unique and updates the file name if needed
    /// </summary>
    private void ValidateAndUpdateID()
    {
        string assetPath = AssetDatabase.GetAssetPath(this);
        string directory = Path.GetDirectoryName(assetPath);

        string[] guids = AssetDatabase.FindAssets("t:CardScriptable", new[] { directory });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CardScriptable card = AssetDatabase.LoadAssetAtPath<CardScriptable>(path);

            if (card != this && card.ID == ID)
            {
                Debug.LogError($"ID {ID} is already in use by another CardScriptable with name {card.CardName} in the same directory.\nPlease choose a unique ID.");
                ID = _prevID;
                return;
            }
        }

        _prevID = ID;

        // Update file name
        string newFilename = $"Card_{ID}.asset";

        AssetDatabase.RenameAsset(assetPath, newFilename);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Card ID updated to {ID} and asset file renamed.");
    }

    // Validator for ConditionalEffects
    private bool HasConditions(List<CardConditionData> conditions)
    {
        return conditions != null && conditions.Count > 0;
    }

    // Validator for ValidTargets
    private bool HasValidTargets(List<string> targets)
    {
        return targets != null && targets.Count > 0;
    }
}