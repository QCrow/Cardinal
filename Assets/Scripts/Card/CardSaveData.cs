using System.Collections.Generic;

public class CardSaveData : SlotContentSaveData
{
    public int ID;

    public int CycleValue;

    public Dictionary<ModifierPersistenceType, Dictionary<CardModifierType, int>> Modifiers = new();

    public CardSaveData(int guid, int row, int col, int id, int cycleValue, Dictionary<ModifierPersistenceType, Dictionary<CardModifierType, int>> modifiers) : base(guid, row, col)
    {
        ID = id;
        CycleValue = cycleValue;
        Modifiers = modifiers;
    }
}