using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomRarityUtil
{
    private static readonly System.Random _random = new();

    // Get a random rarity based on the current level's rarity weights
    private static RarityType GetRandomRarity(
        int level, Dictionary<int, Dictionary<RarityType, int>> levelRarityWeights)
    {
        if (!levelRarityWeights.ContainsKey(level))
        {
            Debug.LogError($"No rarity weights found for level {level}. Using default weights.");
            return RarityType.Common;
        }

        Dictionary<RarityType, int> rarityWeights = levelRarityWeights[level];
        int totalWeight = rarityWeights.Values.Sum();
        int randomValue = _random.Next(0, totalWeight);
        int weightSum = 0;

        foreach (var rarity in rarityWeights)
        {
            weightSum += rarity.Value;
            if (randomValue < weightSum)
            {
                return rarity.Key;
            }
        }

        return RarityType.Common;
    }

    // Generate a list of card IDs by rarity, considering the current level's rarity weights
    public static List<int> GenerateCardIDsByRarity(
        Dictionary<RarityType, List<int>> _rarityCards,
        Dictionary<int, Dictionary<RarityType, int>> levelRarityWeights,
        int level,
        int numberOfCards,
        bool allowDuplicates)
    {
        List<int> generatedCardIDs = new();            // Store selected card IDs
        HashSet<int> uniqueCardTracker = new();       // Track selected IDs if duplicates are not allowed

        for (int i = 0; i < numberOfCards; i++)
        {
            // Get a random rarity based on the current level's weights
            RarityType rarity = GetRandomRarity(level, levelRarityWeights);

            // Ensure the rarity has available cards, or try another
            while (!_rarityCards.ContainsKey(rarity) || _rarityCards[rarity].Count == 0)
            {
                Debug.LogWarning($"No cards left for rarity {rarity}. Trying a different rarity.");
                rarity = GetRandomRarity(level, levelRarityWeights);

                // Stop if all rarities are exhausted
                if (_rarityCards.All(r => r.Value.Count == 0))
                {
                    Debug.LogError("No cards left in any rarity. Stopping generation.");
                    return generatedCardIDs;
                }
            }

            // Select a random card from the available cards of the chosen rarity
            int randomIndex = _random.Next(0, _rarityCards[rarity].Count);
            int cardID = _rarityCards[rarity][randomIndex];

            // If duplicates are not allowed, ensure the card ID is unique
            if (!allowDuplicates)
            {
                if (uniqueCardTracker.Contains(cardID))
                {
                    i--;  // Retry the current iteration to get a unique card
                    continue;
                }
                uniqueCardTracker.Add(cardID);  // Track the selected ID
            }

            // Add the selected card ID to the result
            generatedCardIDs.Add(cardID);
        }
        return generatedCardIDs;
    }
}
