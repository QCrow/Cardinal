using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class SerializableEffect
{
    public EffectType Keyword;

    [ShowIf(nameof(IsKeywordApply))]
    [Tooltip("Type of the applied modifier.")]
    public ModifierType Modifier;

    [ShowIf(nameof(IsKeywordAddCard))]
    [Tooltip("The ID of the card to add.")]
    public int CardID;

    [ShowIf(nameof(IsKeywordApplyOrTempDamageUpOrAddCard))]
    public int Value = 1;

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

    private bool IsKeywordApplyOrTempDamageUpOrAddCard()
    {
        return Keyword == EffectType.Apply || Keyword == EffectType.TempDamageUp || Keyword == EffectType.AddCard;
    }

    private bool IsKeywordApplyOrDestroy()
    {
        return Keyword == EffectType.Apply || Keyword == EffectType.Destroy;
    }

    private bool IsTargetedAndKeywordApplyOrDestroy()
    {
        return IsTargeted && (Keyword == EffectType.Apply || Keyword == EffectType.Destroy);
    }

    private bool IsKeywordAddCard()
    {
        return Keyword == EffectType.AddCard;
    }
}