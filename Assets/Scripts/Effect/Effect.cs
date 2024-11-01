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
    public virtual void Revert(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Revert();
        }
    }
    public virtual void ModifyPotency(int amount) { }
}


public class AddCardModifierEffect : Effect
{
    private readonly CardModifierType _modifierType;
    private EffectValue _value;

    private readonly bool _isTargeted;
    private readonly Target _target;

    public AddCardModifierEffect(Card card, CardModifierType modifierType, EffectValue value, bool isTargeted = false, Target target = null) : base(card)
    {
        if (modifierType is CardModifierType.None)
        {
            throw new System.ArgumentException("ModifierType cannot be None.");
        }
        _modifierType = modifierType;
        _value = value;
        _isTargeted = isTargeted;
        _target = target;
    }

    public override void Apply()
    {
        if (_isTargeted)
        {
            List<Card> cards = _target.GetAvailableCardTargets(_card);
            Debug.Log(cards.Count);
            cards.ForEach(target => target.AddModifier(_modifierType, _value.GetValue(_card), _value.isPermanent));
        }
        else
        {
            _card.AddModifier(_modifierType, _value.GetValue(_card), _value.isPermanent);
        }
    }

    public override void Revert()
    {
        if (_isTargeted)
        {
            _target.GetAvailableCardTargets(_card).ForEach(target => target.RemoveModifier(_modifierType, _value.GetValue(_card), _value.isPermanent));
        }
        else
        {
            _card.RemoveModifier(_modifierType, _value.GetValue(_card), _value.isPermanent);
        }
    }

    public override void ModifyPotency(int amount)
    {
        _value.BaseValue += amount;
    }
}

public class AddSlotModifierEffect : Effect
{
    private readonly SlotModifierType _modifierType;
    private EffectValue _value;

    private readonly bool _isTargeted;
    private readonly Target _target;

    public AddSlotModifierEffect(Card card, SlotModifierType modifierType, EffectValue value, bool isTargeted = false, Target target = null) : base(card)
    {
        if (modifierType is SlotModifierType.None)
        {
            throw new System.ArgumentException("ModifierType cannot be None.");
        }
        _modifierType = modifierType;
        _value = value;
        _isTargeted = isTargeted;
        _target = target;
    }

    public override void Apply()
    {
        if (_isTargeted)
        {
            _target.GetAvailableSlotTargets(_card).ForEach(target =>
            {
                target.AddModifier(_modifierType, _value.GetValue(_card), _value.isPermanent);
            });
        }
        else
        {
            _card.Slot.AddModifier(_modifierType, _value.GetValue(_card), _value.isPermanent);
        }
    }

    public override void Revert()
    {
        if (_isTargeted)
        {
            _target.GetAvailableSlotTargets(_card).ForEach(target =>
            {
                target.RemoveModifier(_modifierType, _value.GetValue(_card), _value.isPermanent);
            });
        }
        else
        {
            _card.Slot.RemoveModifier(_modifierType, _value.GetValue(_card), _value.isPermanent);
        }
    }

    public override void ModifyPotency(int amount)
    {
        _value.BaseValue += amount;
    }
}

public class DestroyEffect : Effect
{
    private readonly bool _isTargeted;
    private readonly Target _target;

    public DestroyEffect(Card card, bool isTargeted = false, Target target = null) : base(card)
    {
        _isTargeted = isTargeted;
        _target = target;
    }

    public override void Apply()
    {
        if (_isTargeted)
        {
            _target.GetAvailableCardTargets(_card).ForEach(target => target.Destroy());
        }
        else
        {
            _card.Destroy();
        }
    }

    public override void Revert()
    {
        throw new System.NotImplementedException("Currently, DestroyEffect cannot be reverted.");

        // if (_isTargeted)
        // {
        //     _target.GetAvailableTargets(_card).ForEach(target => target.ResetTemporaryState());
        // }
        // else
        // {
        //     _card.ResetTemporaryState();
        // }
    }
}

public class AddCardEffect : Effect
{
    private readonly int _cardID;

    public AddCardEffect(Card card, int cardID) : base(card)
    {
        _cardID = cardID;
    }

    public override void Apply()
    {
        CardManager.Instance.AddCardPermanently(_cardID);
    }

    public override void Revert()
    {
        throw new System.NotImplementedException("Currently, AddCardEffect cannot be reverted.");
    }
}

public class TransformEffect : Effect
{
    private readonly int _cardID;
    private readonly bool _isPermanent;
    private readonly bool _isTargeted;
    private readonly Target _target;

    public TransformEffect(Card card, EffectValue effectValue, bool isTargeted, Target target) : base(card)
    {
        _cardID = effectValue.GetValue(card);
        _isPermanent = effectValue.isPermanent;
        _isTargeted = isTargeted;
        _target = target;
    }

    public override void Apply()
    {
        if (_isTargeted)
        {
            _target.GetAvailableCardTargets(_card).ForEach(target =>
            {
                Card newCard = target.Transform(_cardID, _isPermanent);
                newCard.ApplyEffect(TriggerType.WhileInPlay);
            }
        );
        }
        else
        {
            Card newCard = _card.Transform(_cardID, _isPermanent);
            Debug.Log(newCard.ID);
            newCard.ApplyEffect(TriggerType.WhileInPlay);
        }
    }

    public override void Revert()
    {
        throw new System.NotImplementedException();
    }
}

public class GainPermanentDamageAndResetEffect : Effect
{
    private readonly int _value;
    private int _count = 0;
    private readonly Target _target;

    public GainPermanentDamageAndResetEffect(Card card, EffectValue effectValue, Target target) : base(card)
    {
        _value = effectValue.GetValue(card);
        _target = target;
    }

    public override void Apply()
    {
        bool exists = _target.GetAvailableCardTargets(_card).Count > 0;
        if (exists)
        {
            _count++;
            _card.AddModifier(CardModifierType.Strength, _value, true);
        }
        else
        {
            Revert();
        }
    }

    public override void Revert()
    {
        _card.RemoveModifier(CardModifierType.Strength, _value * _count, true);
        _count = 0;
    }
}