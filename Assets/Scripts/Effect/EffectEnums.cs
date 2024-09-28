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
    None, // Only used for default value
    Apply,
    Destroy,
    AddCard,
    Transform
}

public enum ModifierType
{
    None, // Only used for default value
    Strength,
    Weakness,
    MultiStrike
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
    None, // Only used for default value
    Exists,
    Minimum,
    Count
}