public enum TriggerType
{
    None, // Only used for default value
    OnDeploy,
    WhileInPlay,
    OnAttack,
    OnDeath
}

public enum EffectType
{
    None,
    Apply,
    TempDamageUp,
    Destroy,
    AddCard,
    Transform
}

public enum ModifierType
{
    None, // Only used for default value
    Strength,
    Weakness,
    TempDamageUp
}

public enum PositionType
{
    None, // Only used for default value
    Front,
    Middle,
    Back,
    Center,
    Diagonal,
    Corner,
    Edge
}

public enum CheckType
{
    Exists,
    Minimum,
    Count
}