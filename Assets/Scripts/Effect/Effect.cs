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


public class AddModifierEffect : Effect
{
    private readonly ModifierType _modifierType;
    private EffectValue _value;

    private readonly bool _isTargeted;
    private readonly Target _target;

    public AddModifierEffect(Card card, ModifierType modifierType, EffectValue value, bool isTargeted = false, Target target = null) : base(card)
    {
        _modifierType = modifierType;
        _value = value;
        _isTargeted = isTargeted;
        _target = target;
    }

    public override void Apply()
    {
        if (_isTargeted)
        {
            _target.GetAvailableTargets(_card).ForEach(target => target.AddModifier(_modifierType, _value.GetValue(_card), _value.isPermanent));
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
            _target.GetAvailableTargets(_card).ForEach(target => target.RemoveModifier(_modifierType, _value.GetValue(_card), _value.isPermanent));
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
            _target.GetAvailableTargets(_card).ForEach(target => target.Destroy());
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
        CardManager.Instance.AddCard(_cardID);
    }

    public override void Revert()
    {
        throw new System.NotImplementedException("Currently, AddCardEffect cannot be reverted.");
    }
}