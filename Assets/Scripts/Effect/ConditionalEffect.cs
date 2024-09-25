using UnityEngine;
using System.Linq;

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
    public abstract string GenerateDescription();
}

public class ConstantEffect : ConditionalEffect
{
    public ConstantEffect(Card card, Effect effect) : base(card, effect) { }

    public override void ApplyEffect() { _effect.Apply(); }
    public override void RevertEffect() { _effect.Revert(); }
    public override string GenerateDescription() { return _effect.GenerateDescription(); }
}

public class PositionEffect : ConditionalEffect
{
    public PositionType Position;

    public PositionEffect(Card card, Effect effect, PositionType position) : base(card, effect)
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

    public override string GenerateDescription()
    {
        string description = "";
        switch (Position)
        {
            case PositionType.Front:
                description = "Melee: ";
                break;
            case PositionType.Back:
                description = "Ranged: ";
                break;
            default:
                break;
        }
        description += _effect.GenerateDescription();
        return description;
    }
}

public class NextToEffect : ConditionalEffect
{
    public Selector NextTo;

    public NextToEffect(Card card, Effect effect, Selector nextTo) : base(card, effect)
    {
        NextTo = nextTo;
    }

    public override void ApplyEffect()
    {
        switch (NextTo.Check)
        {
            case SelectorCheckType.Exists:
                if (Card.Slot.Neighbors.Exists(slot => slot != null && slot.Card != null && NextTo.IsMatch(slot.Card)))
                {
                    _effect.Apply();
                }
                break;
            case SelectorCheckType.Minimum:
                if (Card.Slot.Neighbors.Count(slot => slot != null && slot.Card != null && NextTo.IsMatch(slot.Card)) >= NextTo.Minimum)
                {
                    _effect.Apply();
                }
                break;
            case SelectorCheckType.Count:
                int count = Card.Slot.Neighbors.Count(slot => slot != null && slot.Card != null && NextTo.IsMatch(slot.Card));
                _effect.Apply(count);
                break;
        }
    }

    public override void RevertEffect()
    {
        switch (NextTo.Check)
        {
            case SelectorCheckType.Exists:
                if (Card.Slot.Neighbors.Exists(slot => slot != null && slot.Card != null && NextTo.IsMatch(slot.Card)))
                {
                    _effect.Revert();
                }
                break;
            case SelectorCheckType.Minimum:
                if (Card.Slot.Neighbors.Count(slot => slot != null && slot.Card != null && NextTo.IsMatch(slot.Card)) >= NextTo.Minimum)
                {
                    _effect.Revert();
                }
                break;
            case SelectorCheckType.Count:
                int count = Card.Slot.Neighbors.Count(slot => slot != null && slot.Card != null && NextTo.IsMatch(slot.Card));
                _effect.Revert(count);
                break;
        }
    }

    public override string GenerateDescription()
    {
        string description = "";
        switch (NextTo.Check)
        {
            case SelectorCheckType.Exists:
                description = "If next to ";
                break;
            case SelectorCheckType.Minimum:
                description = $"If next to at least {NextTo.Minimum} ";
                break;
            default:
                break;
        }
        switch (NextTo.Type)
        {
            case SelectorType.ID:
                description += $"{CardManager.Instance.GetCardScriptableByID(NextTo.ID).Name}:";
                break;
            case SelectorType.Name:
                description += $"{NextTo.Name}:";
                break;
            case SelectorType.Trait:
                description += $"{NextTo.Trait}:";
                break;
            default:
                break;
        }

        description += _effect.GenerateDescription();
        return description;
    }
}