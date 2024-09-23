public abstract class ConditionalEffect
{
    public Card Card; // Card that the effect is attached to
    public abstract void ApplyEffect();
}

public class ConstantEffect : ConditionalEffect
{
    public override void ApplyEffect()
    {
        // Apply the effect
    }
}