using UnityEngine;

public class Card : MonoBehaviour
{
    public RarityType Rarity;
    public string Name;
    public ClassType Class;
    public TraitType Type;

    public int Attack;

    public TriggerType EffectTrigger;
    public ConditionalEffect ConditionalEffect;

    public Card(RarityType rarity, string name, ClassType cardClass, TraitType type, int attack, TriggerType effectTrigger, ConditionalEffect effect)
    {
        Rarity = rarity;
        Name = name;
        Class = cardClass;
        Type = type;
        Attack = attack;
        EffectTrigger = effectTrigger;
        ConditionalEffect = effect;
    }
}