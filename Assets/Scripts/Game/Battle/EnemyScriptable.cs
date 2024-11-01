using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy", order = 0)]
public class EnemyScriptable : ScriptableObject
{
    public int MaxHealth;

    [TextArea(3, 10)]
    public string Description;

    public enum EnemyEffectType
    {
        None,
        CenterNoDamage,
        TopLeftNoDamage
    }

    public EnemyEffectType EffectType;

    private EnemyEffect _effect;

    public void Initialize()
    {
        switch (EffectType)
        {
            case EnemyEffectType.None:
                _effect = new NoEnemyEffect();
                break;
            case EnemyEffectType.CenterNoDamage:
                _effect = new CenterNoDamageEnemyEffect();
                break;
            case EnemyEffectType.TopLeftNoDamage:
                _effect = new TopLeftNoDamageEnemyEffect();
                break;
        }
    }

    public void ApplyEffect()
    {
        _effect.Apply();
    }
}