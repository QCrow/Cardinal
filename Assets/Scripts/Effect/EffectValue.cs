// using System;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public enum VariableType
// {
//     Target
// }

// [Serializable]
// public class EffectValue
// {
//     public bool IsVariable = false;
//     public bool IsPermanent = false;
//     public ModifierPersistenceType PersistenceType = ModifierPersistenceType.Turn;

//     [ShowIf(nameof(IsVariable))]
//     public VariableType VariableType = VariableType.Target;

//     [ShowIf(nameof(IsVariableAndTarget))]
//     [HideLabel]
//     public Target Target;

//     public int BaseValue = 1;

//     public int GetValue(CardInstance card)
//     {
//         return IsVariable ? GetVariableValue(card) * BaseValue : BaseValue;
//     }

//     private int GetVariableValue(CardInstance card)
//     {
//         return VariableType switch
//         {
//             VariableType.Target => Target.GetAvailableCardTargets(card).Count,
//             _ => 0
//         };
//     }

//     private bool IsVariableAndTarget()
//     {
//         return IsVariable && VariableType == VariableType.Target;
//     }
// }