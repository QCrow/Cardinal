using System;
using System.Collections.Generic;

public static class CardLibrary
{
    // Dictionary storing card IDs grouped by rarity type
    private static Dictionary<CardRarityType, List<int>> _cardsByRarity = new();
    public static Dictionary<CardRarityType, List<int>> CardsByRarity => _cardsByRarity;

    // Registry of card creation functions
    private static Dictionary<int, Func<AbstractCard>> _cardRegistry = new();

    // Static constructor to initialize the library
    static CardLibrary()
    {
        RegisterCards();
        PopulateCardsByRarity();
    }

    private static void RegisterCards()
    {
        // Register all cards with their ID and creation function
        _cardRegistry[1] = () => new JourneymanCard();
        _cardRegistry[2] = () => new ApprenticeCard();
        _cardRegistry[3] = () => new ScoutCard();
    }

    private static void PopulateCardsByRarity()
    {
        // Clear the rarity dictionary to ensure it's fresh
        _cardsByRarity.Clear();

        // Iterate over all registered cards and group by rarity
        foreach (var entry in _cardRegistry)
        {
            int cardId = entry.Key;
            AbstractCard card = entry.Value(); // Instantiate the card to access its properties

            if (!_cardsByRarity.ContainsKey(card.Rarity))
            {
                _cardsByRarity[card.Rarity] = new List<int>();
            }

            _cardsByRarity[card.Rarity].Add(cardId);
        }
    }

    public static AbstractCard GetCard(int id)
    {
        if (!_cardRegistry.ContainsKey(id))
        {
            throw new ArgumentException($"Card ID {id} is not registered in the library.");
        }

        return _cardRegistry[id]();
    }
}
