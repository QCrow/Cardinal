using System;

public static class ModifierFactory
{
    public static Modifier CreateModifier(ModifierType type, int amount)
    {
        switch (type)
        {
            case ModifierType.Everlasting:
                return new EverlastingModifier(amount);
            default:
                throw new NotSupportedException($"Modifier type '{type}' is not supported.");
        }
    }
}