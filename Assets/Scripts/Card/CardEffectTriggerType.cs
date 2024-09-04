/// <summary>
/// Represents the type of event that triggers the card effects.
/// </summary>
public enum CardEffectTriggerType
{
    OnPlay,
    OnTurnEnd,
    OnDraw,
    OnDiscard,
    OnDestroy,
    WhileInHand,
    WhileInPlay
}