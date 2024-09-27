using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public abstract class ConditionalEffect
{
    public Card Card; // Card that the effect is attached to
    protected Effect _effect;

    public ConditionalEffect(Card card, Effect effect)
    {
        Card = card;
        _effect = effect;
    }

    public abstract void ApplyEffect();
    public abstract void RevertEffect();
}

public class ConstantEffect : ConditionalEffect
{
    public ConstantEffect(Card card, Effect effect) : base(card, effect) { }

    public override void ApplyEffect() { _effect.Apply(); }
    public override void RevertEffect() { _effect.Revert(); }
}

public class PositionCondition : ConditionalEffect
{
    public PositionType Position;

    public PositionCondition(Card card, Effect effect, PositionType position) : base(card, effect)
    {
        Position = position;
    }

    public override void ApplyEffect()
    {
        if (Card.Slot.IsPosition(Position))
        {
            _effect.Apply();
        }
    }

    public override void RevertEffect()
    {
        if (Card.Slot.IsPosition(Position))
        {
            _effect.Revert();
        }
    }
}

public class TargetWithPropertyCondition : ConditionalEffect
{
    public Target TargetField;
    private CheckType _check;
    private int _minimum;

    public TargetWithPropertyCondition(Card card, Effect effect, Target targetField, CheckType check, int minimum) : base(card, effect)
    {
        TargetField = targetField;
        _check = check;
        _minimum = minimum;
    }

    public override void ApplyEffect()
    {
        List<Card> targets = TargetField.GetAvailableTargets(Card);
        switch (_check)
        {
            case CheckType.Exists:
                if (targets.Count > 0)
                {
                    _effect.Apply();
                }
                break;
            case CheckType.Minimum:
                if (targets.Count >= _minimum)
                {
                    _effect.Apply();
                }
                break;
            case CheckType.Count:
                _effect.Apply(targets.Count);
                break;
        }
    }

    public override void RevertEffect()
    {
        List<Card> targets = TargetField.GetAvailableTargets(Card);
        switch (_check)
        {
            case CheckType.Exists:
                if (targets.Count > 0)
                {
                    _effect.Revert();
                }
                break;
            case CheckType.Minimum:
                if (targets.Count >= _minimum)
                {
                    _effect.Revert();
                }
                break;
            case CheckType.Count:
                _effect.Revert(targets.Count);
                break;
        }
    }
}