using System.Collections.Generic;

public class ApprenticeCard : AbstractCard
{
    private static readonly Dictionary<string, int> V =
    new()
    {
        {"Cooldown", 1}
    };

    private static readonly HashSet<string> T = new()
    {
        "Skill"
    };

    public ApprenticeCard() : base(2, CardRarityType.Starter, V, T, true) { }

    public override void ActivateCardEffect(TriggerType trigger, CardInstance instance)
    {
        if (trigger is TriggerType.OnActivation)
        {
            if (instance.Cooldown > 0)
            {
                return;
            }
            instance.Cooldown = V["Cooldown"];
            instance.CurrentSlot.AddModifier(SlotModifierType.Assistance, 1, ModifierPersistenceType.Turn);
        }
    }
}