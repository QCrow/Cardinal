public abstract class Effect
{
    public ConditionalEffect ConditionalEffect;

    public abstract void Apply();
    public abstract void Revert();
}