using System.Collections.Generic;

[System.Serializable]
public class CardConditionData
{
    public string Type;
    public CardEffectTriggerType Trigger;
    public List<CardEffectData> Effects;
    public int Value;
}