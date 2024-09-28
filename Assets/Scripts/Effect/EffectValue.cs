using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum VariableType
{
    Target
}

[Serializable]
public class EffectValue
{
    public bool IsVariable = false;
    public bool isPermanent = false;

    [ShowIf(nameof(IsVariable))]
    public VariableType VariableType = VariableType.Target;

    [ShowIf(nameof(IsVariableAndTarget))]
    [HideLabel]
    public Target Target;

    public int BaseValue = 1;

    public int GetValue(Card card)
    {
        return IsVariable ? GetVariableValue(card) * BaseValue : BaseValue;
    }

    private int GetVariableValue(Card card)
    {
        return VariableType switch
        {
            VariableType.Target => Target.GetAvailableTargets(card).Count,
            _ => 0
        };
    }

    private bool IsVariableAndTarget()
    {
        return IsVariable && VariableType == VariableType.Target;
    }
}