using System;
using System.Linq;
using System.Collections.Generic;

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

    public List<Card> GetAvailableTargets(Card src)
    {
        List<Card> targets = new();
        switch (TargetRange)
        {
            case TargetRangeType.Row:
                List<Slot> slots = Board.Instance.GetRow(src.Slot.Row);
                List<Card> cards = slots.ConvertAll(slot => slot.Card).Where(card => card != null).ToList();
                targets.AddRange(cards);
                break;
            default:
                break;
        }

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