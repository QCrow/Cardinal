using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class Slot : SerializedMonoBehaviour
{
    public GameObject ContentContainer => transform.GetChild(0).gameObject;

    #region Position
    public int Row { get; private set; }
    public int Col { get; private set; }
    public void Initialize(int row, int col)
    {
        Row = row;
        Col = col;
    }
    public List<Slot> Neighbors => Board.Instance.GetAdjacentSlots(this);

    public Slot Up => Board.Instance.GetSlotAtPosition(Row - 1, Col);
    public Slot Down => Board.Instance.GetSlotAtPosition(Row + 1, Col);
    public Slot Left => Board.Instance.GetSlotAtPosition(Row, Col - 1);
    public Slot Right => Board.Instance.GetSlotAtPosition(Row, Col + 1);
    #endregion

    public SlotContent Content;

    [SerializeField]
    private Dictionary<ModifierPersistenceType, Dictionary<SlotModifierType, int>> _modifiers = new();

    public bool IsPosition(PositionType position)
    {
        // Mobilization modifier allows the slot to be considered as any position
        if (GetModifierValue(SlotModifierType.Assistance) > 0)
        {
            return true;
        }

        return position switch
        {
            PositionType.Front => Col == 0,
            PositionType.Middle => Col == 1,
            PositionType.Back => Col == 2,
            PositionType.Center => Row == 1 && Col == 1,
            _ => false,
        };
    }

    #region Modifiers
    /// <summary>
    /// Gets the total modifier value of a specific type, considering all persistence levels.
    /// </summary>
    /// <param name="type">The type of modifier to get the value of.</param>
    public int GetModifierValue(SlotModifierType type)
    {
        int total = 0;
        foreach (var persistenceDict in _modifiers.Values)
        {
            if (persistenceDict.TryGetValue(type, out int value))
            {
                total += value;
            }
        }
        return total;
    }

    /// <summary>
    /// Resets all card's modifiers under a specified persistence level.
    /// </summary>
    /// <param name="persistence"> The persistence level to reset.</param>
    public void ResetSlotModifierState(ModifierPersistenceType persistence)
    {
        // Determine which modifier persistence levels need to be cleared
        List<ModifierPersistenceType> persistencesToClear = persistence switch
        {
            ModifierPersistenceType.Turn => new List<ModifierPersistenceType>
        {
                ModifierPersistenceType.Turn
        },
            ModifierPersistenceType.Battle => new List<ModifierPersistenceType>
        {
            ModifierPersistenceType.Turn,
            ModifierPersistenceType.Battle
        },
            ModifierPersistenceType.Permanent => new List<ModifierPersistenceType>
        {
            ModifierPersistenceType.Turn,
            ModifierPersistenceType.Battle,
            ModifierPersistenceType.Permanent
        },
            _ => new List<ModifierPersistenceType>() // Default case, if needed
        };

        // Clear the determined modifier persistence levels
        foreach (var p in persistencesToClear)
        {
            ClearModifiers(p);
        }
    }

    /// <summary>
    /// Adds a modifier to the card with a specified persistence.
    /// </summary>
    /// <param name="type">The type of modifier.</param>
    /// <param name="amount">The amount of the modifier.</param>
    /// <param name="persistence">The persistence level of the modifier.</param>
    public void AddModifier(SlotModifierType type, int amount, ModifierPersistenceType persistence)
    {
        if (!_modifiers.ContainsKey(persistence))
        {
            _modifiers[persistence] = new Dictionary<SlotModifierType, int>();
        }

        if (_modifiers[persistence].ContainsKey(type))
        {
            _modifiers[persistence][type] += amount;
        }
        else
        {
            _modifiers[persistence][type] = amount;
        }
    }

    /// <summary>
    /// Removes a modifier from the card based on its type and persistence.
    /// </summary>
    /// <param name="type">The type of modifier.</param>
    /// <param name="amount">The amount to remove.</param>
    /// <param name="persistence">The persistence level of the modifier.</param>
    public void RemoveModifier(SlotModifierType type, int amount, ModifierPersistenceType persistence)
    {
        if (_modifiers.ContainsKey(persistence) && _modifiers[persistence].ContainsKey(type))
        {
            _modifiers[persistence][type] -= amount;
            if (_modifiers[persistence][type] <= 0)
            {
                _modifiers[persistence].Remove(type);
                if (_modifiers[persistence].Count == 0)
                {
                    _modifiers.Remove(persistence);
                }
            }
        }
    }

    public void ClearModifiers(ModifierPersistenceType persistence)
    {
        if (_modifiers.ContainsKey(persistence))
        {
            _modifiers.Remove(persistence);
        }
    }
    #endregion

    #region Serialization
    public SlotSaveData GetSaveData()
    {
        return new SlotSaveData(Row, Col, _modifiers);
    }

    public void LoadSaveData(SlotSaveData saveData)
    {
        _modifiers = saveData.Modifiers;
    }
    #endregion
}