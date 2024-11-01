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

    [ShowIf(nameof(IsKeywordAddCard))]
    [Tooltip("The ID of the card to add.")]
    public int CardID;

    [ShowIf(nameof(IsKeywordApplyOrTempDamageUpOrAddCardOrGainPermanentDamageAndReset))]
    [BoxGroup("Value")]
    [HideLabel]
    public EffectValue Value;

    [BoxGroup("Targeting")]
    [ShowIf(nameof(IsKeywordApplyOrDestroy))]
    public bool IsTargeted = false;

    [BoxGroup("Targeting")]
    [ShowIf(nameof(IsTargetedAndKeywordApplyOrDestroy))]
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

    private bool IsKeywordApplyOrTempDamageUpOrAddCardOrGainPermanentDamageAndReset()
    {
        return Keyword == EffectType.Apply || Keyword == EffectType.AddCard || Keyword == EffectType.GainPermanentDamageAndReset;
    }

    private bool IsKeywordApplyOrDestroy()
    {
        return Keyword == EffectType.Apply || Keyword == EffectType.Destroy || Keyword == EffectType.GainPermanentDamageAndReset;
    }

    private bool IsTargetedAndKeywordApplyOrDestroy()
    {
        return IsTargeted && (Keyword == EffectType.Apply || Keyword == EffectType.Destroy || Keyword == EffectType.GainPermanentDamageAndReset);
    }

    private bool IsKeywordAddCard()
    {
        return Keyword == EffectType.AddCard;
    }
}