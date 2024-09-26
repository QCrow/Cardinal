using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEditor;
using System.IO;

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
    public ClassType Class;
    public TraitType Trait;

    public int BaseAttack;

    public string Description;

    [Space(10)]
    [Title("Card Effect")]
    public bool HasEffect = false;
    [ShowIf("HasEffect", true)]
    public TriggerType Trigger = TriggerType.OnDeploy;

    [ShowIf("HasEffect", true)]
    public ConditionType Condition;

    [ShowIf("@HasEffect && Condition == ConditionType.Position")]
    public PositionType Position;

    [ShowIf("@HasEffect && Condition == ConditionType.TargetWithProperty")]
    public Target TargetWithProperty;

    [ShowIf("HasEffect", true)]
    public EffectType Keyword;

    [BoxGroup("Effect")]
    [ShowIf("@HasEffect && (Keyword == EffectType.Apply)")]
    [Tooltip("Type of the applied modifier.")]
    public ModifierType Modifier;

    [BoxGroup("Effect")]
    [ShowIf("@HasEffect && (Keyword == EffectType.Apply || Keyword == EffectType.TempDamageUp)")]
    [Tooltip("Amount of the applied modifier.")]
    public int Value = 1;

    [BoxGroup("Effect/Targeting")]
    [ShowIf("@HasEffect && (Keyword == EffectType.Apply)")]
    public bool IsTargeted = false;
    [BoxGroup("Effect/Targeting")]
    [ShowIf("@HasEffect && IsTargeted && Keyword == EffectType.Apply")]
    public Target Target;

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