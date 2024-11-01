using Sirenix.OdinInspector.Editor;

public enum TriggerType
{
    None, // Only used for default value
    OnDeploy,
    PrioWhileInPlay,
    WhileInPlay,
    BeforeAttack,
    OnAttack,
    AfterAttack,
    OnDeath,
    OnMove
}

public enum EffectType
{
    None, // Only used for default value
    Apply,
    Destroy,
    AddCard,
    Transform,
    GainPermanentDamageAndReset,
    Mono
}

public enum CardModifierType
{
    None, // Only used for default value
    Strength,
    Weakness,
    MultiStrike
}

public enum SlotModifierType
{
    None, // Only used for default value
    Mobilization,
    NoDamage
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