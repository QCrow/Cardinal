using System.Collections.Generic;

public class SwordOathApprenticeCard : AbstractCard
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

    public SwordOathApprenticeCard() : base(16, CardRarityType.Common, V, T, true) { }

    public override void ActivateCardEffect(TriggerType trigger, CardInstance instance)
    {

    }
}