using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class SerializableEffect
{
    public EffectType Keyword;

    public enum ApplyOptions
    {
        ToCard,
        ToSlot
    }

    [ShowIf(nameof(IsKeywordApply))]
    public ApplyOptions ApplyOption;

    [ShowIf(nameof(IsKeywordApplyAndToCard))]
    [Tooltip("Type of the applied modifier.")]
    public CardModifierType CardModifier;

    [ShowIf(nameof(IsKeywordApplyAndToSlot))]
    [Tooltip("Type of the applied modifier.")]
    public SlotModifierType SlotModifier;

    [ShowIf(nameof(HasValueCheck))]
    [BoxGroup("Value")]
    [HideLabel]
    public EffectValue Value;

    [BoxGroup("Targeting")]
    [ShowIf(nameof(IsTargetedCheck))]
    public bool IsTargeted = false;

    [BoxGroup("Targeting")]
    [ShowIf(nameof(TargetCheck))]
    public Target Target;

    private bool IsKeywordApply()
    {
        return Keyword == EffectType.Apply;
    }

    private bool IsKeywordApplyAndToCard()
    {
        return Keyword == EffectType.Apply && ApplyOption == ApplyOptions.ToCard;
    }

    private bool IsKeywordApplyAndToSlot()
    {
        return Keyword == EffectType.Apply && ApplyOption == ApplyOptions.ToSlot;
    }

    private bool HasValueCheck()
    {
        return Keyword == EffectType.Apply || Keyword == EffectType.RemoveCard || Keyword == EffectType.AddCard || Keyword == EffectType.GainPermanentDamageAndReset || Keyword == EffectType.Transform;
    }

    private bool IsTargetedCheck()
    {
        return Keyword == EffectType.Apply || Keyword == EffectType.RemoveCard || Keyword == EffectType.GainPermanentDamageAndReset || Keyword == EffectType.Transform;
    }

    private bool TargetCheck()
    {
        return IsTargeted && IsTargetedCheck();
    }

    private bool IsKeywordAddCard()
    {
        return Keyword == EffectType.AddCard;
    }
}