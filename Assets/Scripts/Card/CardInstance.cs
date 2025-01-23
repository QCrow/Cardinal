using System.Collections.Generic;

public class CardInstance
{
    // The card instance data, containing base values, tags and logic
    public AbstractCard Template { get; private set; }
    public Slot CurrentSlot { get; set; }

    public int TokenCount;
    public int ChargeCount;
    public int Cooldown;

    // Modifier storage with persistence levels
    private Dictionary<ModifierPersistenceType, Dictionary<CardModifierType, int>> _modifiers = new();

    public CardInstance(AbstractCard template)
    {
        Template = template;
        TokenCount = 0;
        ChargeCount = 0;
        Cooldown = 0;
    }

    /// <summary>
    /// Gets the total modifier value of a specific type, considering all persistence levels.
    /// </summary>
    /// <param name="type">The type of modifier to get the value of.</param>
    public int GetModifierValue(CardModifierType type)
    {
        int total = 0;
        foreach (var dict in _modifiers.Values)
        {
            if (dict.TryGetValue(type, out int value))
            {
                total += value;
            }
        }
        return total;
    }

    public void AddModifier(CardModifierType type, int amount, ModifierPersistenceType persistence)
    {
        if (!_modifiers.ContainsKey(persistence))
        {
            _modifiers[persistence] = new Dictionary<CardModifierType, int>();
        }

        if (_modifiers[persistence].ContainsKey(type))
        {
            _modifiers[persistence][type] += amount;
        }
        else
        {
            _modifiers[persistence][type] = amount;
        }
    }

    /// <summary>
    /// Removes a modifier from the card based on its type and persistence.
    /// </summary>
    /// <param name="type">The type of modifier.</param>
    /// <param name="amount">The amount to remove.</param>
    /// <param name="persistence">The persistence level of the modifier.</param>
    public void RemoveModifier(CardModifierType type, int amount, ModifierPersistenceType persistence)
    {
        if (_modifiers.ContainsKey(persistence) && _modifiers[persistence].ContainsKey(type))
        {
            _modifiers[persistence][type] -= amount;
            if (_modifiers[persistence][type] <= 0)
            {
                _modifiers[persistence].Remove(type);
                if (_modifiers[persistence].Count == 0)
                {
                    _modifiers.Remove(persistence);
                }
            }
        }
    }

    private void ClearModifiers(ModifierPersistenceType persistence)
    {
        if (_modifiers.ContainsKey(persistence))
        {
            _modifiers.Remove(persistence);
        }
    }
}