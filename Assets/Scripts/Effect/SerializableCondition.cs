using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

[System.Serializable]
public class SerializableCondition
{
    [OnValueChanged(nameof(ValidateTrigger))]
    public TriggerType Trigger;

    [OnValueChanged(nameof(ValidateTrigger))]
    public ConditionType Condition;

    [ShowIf(nameof(IsConditionCycle))]
    public int CycleCount;

    [ShowIf(nameof(IsConditionPosition))]
    public PositionType Position;

    [BoxGroup("Condition Targeting")]
    [ShowIf(nameof(IsConditionTargetWithProperty))]
    public Target TargetWithProperty;

    [BoxGroup("Condition Targeting")]
    [ShowIf(nameof(IsConditionTargetWithProperty))]
    public CheckType Check;

    [BoxGroup("Condition Targeting")]
    [ShowIf(nameof(IsConditionTargetWithPropertyAndMinimum))]
    [Tooltip("The minimum number of neighbors that must match the selector for the effect to activate.")]
    public int Minimum;

    [OnValueChanged(nameof(ValidateTrigger))]
    public List<SerializableEffect> Effects;

    // Hides the warning message field in the inspector but keeps it accessible for the InfoBox
    private string _validationWarningMessage;

    /// <summary>
    /// Validates triggers based on condition and effects, updates warning messages.
    /// </summary>
    private void ValidateTrigger()
    {
        _validationWarningMessage = ""; // Clear any previous messages

        if (Trigger == TriggerType.OnAttack) return; // No need to validate 'OnAttack' triggers

        // Cycle trigger validation
        if (Condition == ConditionType.Cycle && Trigger != TriggerType.OnAttack)
        {
            Trigger = TriggerType.OnAttack;
            _validationWarningMessage = "Condition 'Cycle' requires the Trigger to be 'OnAttack'. Automatically set.";
        }

        // Effect-based trigger validation
        if (Effects != null && Effects.Any(effect => effect.Keyword == EffectType.Destroy || effect.Keyword == EffectType.AddCard))
        {
            if (Trigger != TriggerType.OnAttack && Trigger != TriggerType.OnDeath)
            {
                Trigger = TriggerType.OnAttack;
                _validationWarningMessage += "\nEffects containing 'Destroy' or 'AddCard' require the Trigger to be 'OnAttack' or 'OnDeath'. Automatically set.";
            }
        }
    }

    [OnInspectorGUI]
    private void DrawWarningBox()
    {
        if (HasValidationWarning())
        {
            SirenixEditorGUI.WarningMessageBox(_validationWarningMessage);
            if (GUILayout.Button("Dismiss Warning"))
            {
                _validationWarningMessage = ""; // Hide the warning when the button is clicked.
            }
        }
    }

    // Determines if the validation warning should be shown in the inspector
    private bool HasValidationWarning()
    {
        return !string.IsNullOrEmpty(_validationWarningMessage);
    }

    private bool IsConditionCycle()
    {
        return Condition == ConditionType.Cycle;
    }

    private bool IsConditionPosition()
    {
        return Condition == ConditionType.Position;
    }

    private bool IsConditionTargetWithProperty()
    {
        return Condition == ConditionType.TargetWithProperty;
    }

    private bool IsConditionTargetWithPropertyAndMinimum()
    {
        return Condition == ConditionType.TargetWithProperty && Check == CheckType.Minimum;
    }
}
