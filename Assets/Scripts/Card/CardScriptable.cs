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

    [ShowIf(nameof(HasEffectActive))]
    [OnValueChanged("ValidateTrigger")]
    public TriggerType Trigger = TriggerType.OnDeploy;

    [ShowIf(nameof(HasEffectActive))]
    [OnValueChanged("ValidateTrigger")]
    public ConditionType Condition;

    [ShowIf(nameof(IsConditionCycle))]
    public int CycleCount;

    [ShowIf(nameof(IsConditionPosition))]
    public PositionType Position;

    [ShowIf(nameof(IsConditionTargetWithProperty))]
    public Target TargetWithProperty;

    [ShowIf(nameof(IsConditionTargetWithProperty))]
    public CheckType Check;

    [ShowIf(nameof(IsConditionTargetWithPropertyAndMinimum))]
    [Tooltip("The minimum number of neighbors that must match the selector for the effect to activate.")]
    public int Minimum;

    [ShowIf(nameof(HasEffectActive))]
    public EffectType Keyword;

    [BoxGroup("Effect")]
    [ShowIf(nameof(IsKeywordApply))]
    [Tooltip("Type of the applied modifier.")]
    public ModifierType Modifier;

    [BoxGroup("Effect")]
    [ShowIf(nameof(IsKeywordApplyOrTempDamageUp))]
    [Tooltip("Amount of the applied modifier.")]
    public int Value = 1;

    [BoxGroup("Effect/Targeting")]
    [ShowIf(nameof(IsKeywordApplyOrDestroy))]
    public bool IsTargeted = false;

    [BoxGroup("Effect/Targeting")]
    [ShowIf(nameof(IsTargetedAndKeywordApplyOrDestroy))]
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

    /// <summary>
    /// Enforces that if ConditionType is Cycle, Trigger is OnAttack.
    /// </summary>
    private void ValidateTrigger()
    {
        if (Condition == ConditionType.Cycle)
        {
            Trigger = TriggerType.OnAttack;
        }
    }
    #endregion

    #region Helper Methods for Inspector Conditions

    private bool HasEffectActive()
    {
        return HasEffect;
    }

    private bool IsConditionCycle()
    {
        return HasEffect && Condition == ConditionType.Cycle;
    }

    private bool IsConditionPosition()
    {
        return HasEffect && Condition == ConditionType.Position;
    }

    private bool IsConditionTargetWithProperty()
    {
        return HasEffect && Condition == ConditionType.TargetWithProperty;
    }

    private bool IsConditionTargetWithPropertyAndMinimum()
    {
        return HasEffect && Condition == ConditionType.TargetWithProperty && Check == CheckType.Minimum;
    }

    private bool IsKeywordApply()
    {
        return HasEffect && Keyword == EffectType.Apply;
    }

    private bool IsKeywordApplyOrTempDamageUp()
    {
        return HasEffect && (Keyword == EffectType.Apply || Keyword == EffectType.TempDamageUp);
    }

    private bool IsKeywordApplyOrDestroy()
    {
        return HasEffect && (Keyword == EffectType.Apply || Keyword == EffectType.Destroy);
    }

    private bool IsTargetedAndKeywordApplyOrDestroy()
    {
        return HasEffect && IsTargeted && (Keyword == EffectType.Apply || Keyword == EffectType.Destroy);
    }

    #endregion
}
