using System;
using System.Collections.Generic;

public static class CardLibrary
{
    // Dictionary storing card IDs grouped by rarity type
    private static Dictionary<CardRarityType, List<int>> _cardsByRarity = new();
    public static Dictionary<CardRarityType, List<int>> CardsByRarity => _cardsByRarity;


    private static Dictionary<int, Func<AbstractCard>> _cardRegistry = new();
    static CardLibrary()
    {
        RegisterCards();
    }

    private static void RegisterCards()
    {
        _cardRegistry[1] = () => new JourneymanCard();
        _cardRegistry[2] = () => new HerbalistCard();
        _cardRegistry[3] = () => new ScoutCard();
    }

    public static AbstractCard GetCard(int id)
    {
        return _cardRegistry[id]();
    }
}
