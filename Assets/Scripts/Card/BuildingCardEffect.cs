using System.Collections.Generic;

public enum EffectTrigger
{
    ON_BUILD,
    ON_DESTROY,
    ON_TURN_END,
    AURA
}

public enum EffectType
{

}

public class EffectRange
{

}

public class BuildingCardEffect
{
    public EffectTrigger Trigger { get; set; }
    public EffectType Type { get; set; }
    public EffectRange Range { get; set; }

    public List<ColorType> Colors { get; set; }
    public List<int> Values { get; set; }

    public BuildingCardEffect(EffectTrigger trigger, EffectType type, EffectRange range, List<ColorType> colors, List<int> values)
    {
        Trigger = trigger;
        Type = type;
        Range = range;
        Colors = colors;
        Values = values;
    }

    public void Resolve()
    {

    }
}