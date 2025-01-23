using System.Collections.Generic;

public abstract class AbstractCard
{
    public int ID { get; protected set; }
    public CardRarityType Rarity { get; protected set; }
    public Dictionary<string, int> BaseValues { get; protected set; }
    public HashSet<string> Tags { get; protected set; }

    public bool CanBeManuallyActivated { get; protected set; }

    public AbstractCard(int id, CardRarityType rarity, Dictionary<string, int> baseValues, HashSet<string> tags, bool canBeManuallyActivated)
    {
        ID = id;
        Rarity = rarity;
        BaseValues = baseValues;
        Tags = tags;
        CanBeManuallyActivated = canBeManuallyActivated;
    }

    public abstract void ActivateCardEffect(TriggerType trigger, CardInstance instance);
}