public abstract class Modifier
{
    public int Amount;
    public ModifierType Type;
    public Modifier(int amount)
    {
        Amount = amount;
    }
    public virtual void OnApply(Card card, int amount) { }
    public virtual void OnRemove(Card card, int amount) { }
    public virtual void OnEndTurn() { }
}

public class EverlastingModifier : Modifier
{
    public EverlastingModifier(int amount) : base(amount)
    {
        Type = ModifierType.Everlasting;
    }
}