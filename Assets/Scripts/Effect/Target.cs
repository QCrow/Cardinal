using System;
using System.Linq;
using System.Collections.Generic;

public enum TargetRangeType
{
    None, // Only used for default value
    Orthogonal,
    Surrounding,
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
    public TraitType TargetTrait;

    public Target(TargetRangeType targetRange, TraitType targetTrait)
    {
        TargetRange = targetRange;
        TargetTrait = targetTrait;
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
            case TargetRangeType.Row:
                slots = Board.Instance.GetRow(src.Slot.Row);
                break;
            case TargetRangeType.Column:
                slots = Board.Instance.GetColumn(src.Slot.Col);
                break;
            case TargetRangeType.Front:
                slots = Board.Instance.GetRow(0);
                break;
            case TargetRangeType.Middle:
                slots = Board.Instance.GetRow(1);
                break;
            case TargetRangeType.Back:
                slots = Board.Instance.GetRow(2);
                break;
            default:
                break;
        }
        targets = TargetSlotsToCards(slots);

        switch (TargetTrait)
        {
            case TraitType.None:
                return new(); // Return an empty list
            case TraitType.All:
                break;
            default:
                targets.RemoveAll(card => card.Trait != TargetTrait);
                break;
        }
        return targets;
    }
}