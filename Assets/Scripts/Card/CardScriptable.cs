using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEditor;
using System.IO;

[CreateAssetMenu(fileName = "CardScriptable", menuName = "CardScriptable", order = 0)]
public class CardScriptable : SerializedScriptableObject
{
    [JsonIgnore]
    private int _prevID;

    [OnValueChanged("ValidateAndUpdateID")]
    public int ID;
    public string CardName;
    public Dictionary<string, int> Products;
    public List<EffectData> Effects;

    public string PlaceableCondition;

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
}

[System.Serializable]
public class EffectData
{
    public string Keyword;
    public string Trigger;
    public List<int> Values;
    public List<string> Modifiers;
}