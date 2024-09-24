using System.Collections.Generic;
using UnityEngine;
public abstract class Effect
{
    protected Card _card;

    public Effect(Card card)
    {
        _card = card;
    }

    public abstract void Apply();
    public virtual void Apply(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Apply();
        }
    }
    public abstract void Revert();
    public virtual void ModifyPotency(int amount) { }
    public abstract string GenerateDescription();
}

public class TempDamageUpEffect : Effect
{
    private int _damage;

    public TempDamageUpEffect(Card card, int damage) : base(card)
    {
        _damage = damage;
    }

    public override void Apply()
    {
        _card.TempDamage += _damage;
    }

    public override void Revert()
    {
        _card.TempDamage -= _damage;
    }

    public override void ModifyPotency(int amount)
    {
        _damage += amount;
    }

    public override string GenerateDescription()
    {
        return $"+{_damage} temp DMG.";
    }
}

public class AddModifierEffect : Effect
{
    private ModifierType _modifierType;
    private int _amount;

    private bool _isTargeted;
    private Target _target;

    public AddModifierEffect(Card card, ModifierType modifierType, int amount, bool isTargeted = false, Target target = null) : base(card)
    {
        _modifierType = modifierType;
        _amount = amount;
        _isTargeted = isTargeted;
        _target = target;
    }

    public override void Apply()
    {
        if (_isTargeted)
        {
            Debug.Log("Applying targeted modifier");
            List<Card> cards = _target.GetAvailableTargets(_card);
            Debug.Log(cards.Count);
            cards.ForEach(target => target.AddModifier(_modifierType, _amount));
        }
        else
        {
            _card.AddModifier(_modifierType, _amount);
        }
    }

    public override void Revert()
    {
        if (_isTargeted)
        {
            _target.GetAvailableTargets(_card).ForEach(target => target.RemoveModifier(_modifierType, _amount));
        }
        else
        {
            _card.RemoveModifier(_modifierType, _amount);
        }
    }

    public override void ModifyPotency(int amount)
    {
        _amount += amount;
    }

    public override string GenerateDescription()
    {
        return $"+{_amount} {_modifierType.ToString()}";
    }
}