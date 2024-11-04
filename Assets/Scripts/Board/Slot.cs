using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
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

    [SerializeField] private Dictionary<SlotModifierType, int> _permanentModifiers = new();
    [SerializeField] private Dictionary<SlotModifierType, int> _temporaryModifiers = new();

    public bool IsPosition(PositionType position)
    {
        // Mobilization modifier allows the slot to be considered as any position
        if (GetModifierByType(SlotModifierType.Mobilization) > 0)
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
    public void AddModifier(SlotModifierType type, int amount, bool isPermanent)
    {
        if (isPermanent)
        {
            if (!_permanentModifiers.ContainsKey(type))
            {
                _permanentModifiers[type] = amount;
            }
            else
            {
                _permanentModifiers[type] += amount;
            }
        }
        else
        {
            if (!_temporaryModifiers.ContainsKey(type))
            {
                _temporaryModifiers[type] = amount;
            }
            else
            {
                _temporaryModifiers[type] += amount;
            }
        }
    }

    public void RemoveModifier(SlotModifierType type, int amount, bool isPermanent)
    {
        if (isPermanent)
        {
            if (_permanentModifiers.ContainsKey(type))
            {
                _permanentModifiers[type] -= amount;
                if (_permanentModifiers[type] <= 0)
                {
                    _permanentModifiers.Remove(type);
                }
            }
        }
        else
        {
            if (_temporaryModifiers.ContainsKey(type))
            {
                _temporaryModifiers[type] -= amount;
                if (_temporaryModifiers[type] <= 0)
                {
                    _temporaryModifiers.Remove(type);
                }
            }
        }
    }

    public int GetModifierByType(SlotModifierType type)
    {
        return (_permanentModifiers.TryGetValue(type, out int permanentValue) ? permanentValue : 0) +
               (_temporaryModifiers.TryGetValue(type, out int temporaryValue) ? temporaryValue : 0);
    }

    public void ResetTemporaryState()
    {
        _temporaryModifiers.Clear();
    }
    #endregion
}