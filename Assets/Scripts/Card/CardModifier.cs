public abstract class CardModifier
{
    public virtual void OnApply(Card card, int value) { }
    public virtual void OnRemove(Card card, int value) { }
}

public class CleanseCardModifier : CardModifier
{
    public override void OnApply(Card card, int value)
    {
    }

    public override void OnRemove(Card card, int value)
    {
    }
}