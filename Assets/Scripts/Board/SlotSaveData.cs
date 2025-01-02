using System.Collections.Generic;

public class SlotSaveData
{
    public int Row;
    public int Col;
    public Dictionary<ModifierPersistenceType, Dictionary<SlotModifierType, int>> Modifiers = new();

    public SlotSaveData(int row, int col, Dictionary<ModifierPersistenceType, Dictionary<SlotModifierType, int>> modifiers)
    {
        Row = row;
        Col = col;
        Modifiers = modifiers;
    }
}