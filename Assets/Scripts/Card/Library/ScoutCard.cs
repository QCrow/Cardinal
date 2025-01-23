using System.Collections.Generic;

public class ScoutCard : AbstractCard
{
    private static readonly Dictionary<string, int> V =
    new()
    {
        {"Damage", 0},
        {"Damage Scaling", 3}
    };

    private static readonly HashSet<string> T = new()
    {
        "Damage",
        "Scaling",
        "Movement"
    };

    public ScoutCard() : base(3, CardRarityType.Starter, V, T, true) { }

    public override void ActivateCardEffect(TriggerType trigger, CardInstance instance)
    {
        if (trigger is TriggerType.OnMove)
        {
            instance.AddModifier(CardModifierType.Damage, V["Damage Scaling"], ModifierPersistenceType.Turn);
        }

        if (trigger is TriggerType.OnAttack)
        {
            BattleManager.Instance.DealDamage(V["Damage"] + instance.GetModifierValue(CardModifierType.Damage));
        }
    }
}