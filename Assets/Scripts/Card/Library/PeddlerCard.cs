using System.Collections.Generic;

public class PeddlerCard : AbstractCard
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

    public PeddlerCard() : base(11, CardRarityType.Rare, V, T, true) { }

    public override void ActivateCardEffect(TriggerType trigger, CardInstance instance)
    {

    }
}