using System.Collections.Generic;

public abstract class ConditionalEffect
{
    public Card Card; // Card that the effect is attached to
    protected List<Effect> _effects;

    public ConditionalEffect(Card card, List<Effect> effects)
    {
        Card = card;
        _effects = effects;
    }

    public virtual void ApplyEffect()
    {
        foreach (Effect effect in _effects) { effect.Apply(); }
    }
    public virtual void RevertEffect()
    {
        foreach (Effect effect in _effects) { effect.Revert(); }
    }
}

public class ConstantEffect : ConditionalEffect
{
    public ConstantEffect(Card card, List<Effect> effects) : base(card, effects) { }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }
}

public class PositionCondition : ConditionalEffect
{
    public PositionType Position;

    public PositionCondition(Card card, List<Effect> effects, PositionType position) : base(card, effects)
    {
        Position = position;
    }

    public override void ApplyEffect()
    {
        if (Card.CurrentSlot.IsPosition(Position))
        {
            base.ApplyEffect();
        }
    }

    public override void RevertEffect()
    {
        if (Card.CurrentSlot.IsPosition(Position))
        {
            base.RevertEffect();
        }
    }
}

public class TargetWithFilterCondition : ConditionalEffect
{
    public Target TargetField;
    private readonly CheckType _check;
    private readonly int _minimum;

    public TargetWithFilterCondition(Card card, List<Effect> effects, Target targetField, CheckType check, int minimum) : base(card, effects)
    {
        TargetField = targetField;
        _check = check;
        _minimum = minimum;
    }

    public override void ApplyEffect()
    {
        List<Card> targets = TargetField.GetAvailableCardTargets(Card);
        switch (_check)
        {
            case CheckType.Exists:
                if (targets.Count > 0)
                {
                    base.ApplyEffect();
                }
                break;
            case CheckType.Minimum:
                if (targets.Count >= _minimum)
                {
                    base.ApplyEffect();
                }
                break;
            case CheckType.Count:
                foreach (Effect effect in _effects)
                {
                    effect.Apply(targets.Count);
                }
                break;
        }
    }

    public override void RevertEffect()
    {
        List<Card> targets = TargetField.GetAvailableCardTargets(Card);
        switch (_check)
        {
            case CheckType.Exists:
                if (targets.Count > 0)
                {
                    base.RevertEffect();
                }
                break;
            case CheckType.Minimum:
                if (targets.Count >= _minimum)
                {
                    base.RevertEffect();
                }
                break;
            case CheckType.Count:
                foreach (Effect effect in _effects)
                {
                    effect.Revert(targets.Count);
                }
                break;
        }
    }
}

public class CycleCondition : ConditionalEffect
{
    private readonly int _cycleCount;
    public int CycleValue;

    public CycleCondition(Card card, List<Effect> effects, int cycleCount) : base(card, effects)
    {
        card.UpdateCycleValue(cycleCount);
        _cycleCount = cycleCount;
        CycleValue = cycleCount;
    }

    public override void ApplyEffect()
    {
        CycleValue--;
        if (CycleValue <= 0)
        {
            CycleValue = _cycleCount;
            base.ApplyEffect();
        }
        Card.UpdateCycleValue(CycleValue);
    }

    public override void RevertEffect()
    {
        throw new System.NotImplementedException("Effect for CycleCondition cannot be reverted.");
    }

    public void ResetCycle()
    {
        CycleValue = _cycleCount;
        Card.UpdateCycleValue(CycleValue);
    }
}