using System.Collections.Generic;

public class JourneymanCard : AbstractCard
{
    private static readonly Dictionary<string, int> V =
    new()
    {
        { "Damage" , 5 },
        { "Shield" , 4 }
    };

    private static readonly HashSet<string> T = new()
    {
        "Damage",
        "Shield",
        "Front",
        "Back"
    };

    public JourneymanCard() : base(1, CardRarityType.Starter, V, T, false)
    {
    }

    public override void ActivateCardEffect(TriggerType trigger, CardInstance instance)
    {
        if (trigger is TriggerType.OnAttack)
        {
            Slot slot = instance.CurrentSlot;

            if (slot.IsPosition(PositionType.Front))
            {
                BattleManager.Instance.DealDamage(V["Damage"] + instance.GetModifierValue(CardModifierType.Damage));
            }
            if (slot.IsPosition(PositionType.Back))
            {
                BattleManager.Instance.GainShield(V["Shield"] + instance.GetModifierValue(CardModifierType.Shield));
            }
        }
    }
}