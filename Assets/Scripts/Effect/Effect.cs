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
}

public class AddModifierEffect : Effect
{
    private readonly ModifierType _modifierType;
    private int _amount;

    private readonly bool _isTargeted;
    private readonly Target _target;

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
            _target.GetAvailableTargets(_card).ForEach(target => target.AddModifier(_modifierType, _amount));
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