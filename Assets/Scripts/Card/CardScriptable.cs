using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardScriptable : SerializedScriptableObject
{
    [OnValueChanged("ValidateAndUpdateID")]
    [Delayed]
    [Tooltip("Unique identifier for the card. This ID should be unique within the directory.")]
    public int ID;

    [JsonIgnore]
    private int _prevID;

    public string Name;
    public RarityType Rarity;
    public HashSet<TraitType> Traits = new();

    public int BaseAttack;

    public string Description;

    [Space(10)]
    [Title("Card Effect")]
    public bool HasEffect = false;

    [ShowIf(nameof(HasEffectActive))]
    public List<SerializableCondition> Conditions = new();

    private bool HasEffectActive()
    {
        return HasEffect;
    }

    #region Validation
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
                Debug.LogError($"ID {ID} is already in use by another CardScriptable with name {card.Name} in the same directory.\nPlease choose a unique ID.");
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
    #endregion
}
