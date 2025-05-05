using System.Collections.Generic;

public class ShieldGuardCard : AbstractCard
{
    private static readonly Dictionary<string, int> V =
    new()
    {
        {"Damage", 0}
    };

    private static readonly HashSet<string> T = new()
    {
        "Damage"
    };

    public ShieldGuardCard() : base(6, CardRarityType.Common, V, T, true) { }

    public override void ActivateCardEffect(TriggerType trigger, CardInstance instance)
    {

    }
}