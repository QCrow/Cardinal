using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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

    // Message displayed in inspector when validation fails
    [InfoBox("$ValidationWarningMessage", InfoMessageType.Warning, VisibleIf = nameof(HasValidationWarning))]
    public string ValidationWarningMessage;

    /// <summary>
    /// Validates triggers based on condition and effects, updates warning messages.
    /// </summary>
    private void ValidateTrigger()
    {
        ValidationWarningMessage = ""; // Clear any previous messages

        if (Trigger == TriggerType.OnAttack) return; // No need to validate 'OnAttack' triggers

        // Cycle trigger validation
        if (Condition == ConditionType.Cycle && Trigger != TriggerType.OnAttack)
        {
            Trigger = TriggerType.OnAttack;
            ValidationWarningMessage = "Condition 'Cycle' requires the Trigger to be 'OnAttack'. Automatically set.";
        }

        // Effect-based trigger validation
        if (Effects != null && Effects.Any(effect => effect.Keyword == EffectType.Destroy || effect.Keyword == EffectType.AddCard))
        {
            if (Trigger != TriggerType.OnAttack && Trigger != TriggerType.OnDeath)
            {
                Trigger = TriggerType.OnAttack;
                ValidationWarningMessage += "\nEffects containing 'Destroy' or 'AddCard' require the Trigger to be 'OnAttack' or 'OnDeath'. Automatically set.";
            }
        }
    }

    // Determines if the validation warning should be shown in the inspector
    private bool HasValidationWarning()
    {
        return !string.IsNullOrEmpty(ValidationWarningMessage);
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
