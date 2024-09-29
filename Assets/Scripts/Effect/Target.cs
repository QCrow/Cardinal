using System;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public enum TargetRangeType
{
    None, // Only used for default value
    Adjacent,
    Row,
    Column,
    Front,
    Middle,
    Back,
    Center,
    Diagonal,
    Corner,
    Edge,
    Random,
    All
}

[Serializable]
public class Target
{
    public TargetRangeType TargetRange;
    [InlineProperty]
    public Filter TargetFilter;

    public Target(TargetRangeType targetRange, Filter targetFilter)
    {
        TargetRange = targetRange;
        TargetFilter = targetFilter;
    }

    private List<Card> TargetSlotsToCards(List<Slot> slots)
    {
        return slots.ConvertAll(slot => slot.Card).Where(card => card != null).ToList();
    }

    public List<Card> GetAvailableTargets(Card src)
    {
        List<Card> targets = new();
        List<Slot> slots = new();
        switch (TargetRange)
        {
            case TargetRangeType.Adjacent:
                slots = src.Slot.Neighbors;
                break;
            case TargetRangeType.Row:
                slots = Board.Instance.GetRow(src.Slot.Row);
                break;
            case TargetRangeType.Column:
                slots = Board.Instance.GetColumn(src.Slot.Col);
                break;
            case TargetRangeType.Front:
                slots = Board.Instance.GetColumn(0);
                break;
            case TargetRangeType.Middle:
                slots = Board.Instance.GetColumn(1);
                break;
            case TargetRangeType.Back:
                slots = Board.Instance.GetColumn(2);
                break;
            case TargetRangeType.Center:
                slots = Board.Instance.GetRow(1).FindAll(slot => slot.Col == 1);
                break;
            default:
                break;
        }
        targets = TargetSlotsToCards(slots);

        if (TargetFilter != null)
        {
            targets = targets.FindAll(card => TargetFilter.IsMatch(card));
        }

        return targets;
    }
}