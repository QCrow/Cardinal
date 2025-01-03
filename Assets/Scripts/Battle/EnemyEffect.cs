public abstract class EnemyEffect
{
    public abstract void Apply();
}

public class NoEnemyEffect : EnemyEffect
{
    public override void Apply()
    {
    }
}

public class CenterNoDamageEnemyEffect : EnemyEffect
{
    public override void Apply()
    {
        Board.Instance.GetSlotAtPosition(1, 1).AddModifier(SlotModifierType.NoDamage, 1, ModifierPersistenceType.Battle);
    }
}

public class TopLeftNoDamageEnemyEffect : EnemyEffect
{
    public override void Apply()
    {
        Board.Instance.GetSlotAtPosition(0, 0).AddModifier(SlotModifierType.NoDamage, 1, ModifierPersistenceType.Battle);
    }
}