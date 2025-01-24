using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generates card rewards based on rarity weights and level configurations.
/// </summary>
public class CardRewardGenerator
{
    // Configuration containing rarity weights for each level
    private CardGenConfig _randomGenerationConfig;

    private int CardRewardSeed;

    private bool seedInitialized = false;
    /// <summary>
    /// Constructor to initialize the CardRewardGenerator with a given configuration.
    /// </summary>
    /// <param name="randomGenerationConfig">Configuration for card rarity weights per level.</param>
    public CardRewardGenerator(CardGenConfig randomGenerationConfig)
    {
        _randomGenerationConfig = randomGenerationConfig;
    }

    #region Public Methods

    /// <summary>
    /// Generates a list of card reward choices based on the specified level and number of choices.
    /// </summary>
    /// <param name="level">The current level, used to determine rarity weights.</param>
    /// <param name="numberOfChoices">The number of card rewards to generate.</param>
    /// <returns>A list of generated card rewards.</returns>
    public List<CardReward> GenerateCardRewardChoices(int level, int numberOfChoices)
    {
        List<CardReward> rewards = new();

        for (int i = 0; i < numberOfChoices; i++)
        {
            CardReward reward = GenerateCardReward(level);
            rewards.Add(reward);
        }

        return rewards;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Generates a single card reward based on the level and configured rarity weights.
    /// </summary>
    /// <param name="level">The current level, used to determine rarity weights.</param>
    /// <returns>A generated card reward.</returns>
    private CardReward GenerateCardReward(int level)
    {
        if (!seedInitialized)
        {
            CardRewardSeed = GameManager.Instance.Seed;
            seedInitialized = true;
        }
        CardRewardSeed = GameManager.Instance.GetDerivedSeedWithPosition(CardRewardSeed, 104729, 49999);
        Random.InitState(CardRewardSeed);
        // Validate level and adjust if necessary
        if (level >= _randomGenerationConfig.CardRarityWeightsPerLevel.Count)
        {
            Debug.LogError($"No rarity weights found for level {level}. Using the highest level's weights.");
            level = _randomGenerationConfig.CardRarityWeightsPerLevel.Count - 1;
        }

        // Get the rarity weights for the specified level
        var rarityWeights = _randomGenerationConfig.CardRarityWeightsPerLevel[level];

        // Calculate the total weight and select a rarity using a random value
        int totalWeight = rarityWeights.Values.Sum();
        int randomValue = Random.Range(0, totalWeight);
        int weightSum = 0;
        CardRarityType rarity = CardRarityType.Common;

        // Determine the selected rarity based on the random value
        foreach (var pair in rarityWeights)
        {
            weightSum += pair.Value;
            if (randomValue < weightSum)
            {
                rarity = pair.Key;
                break;
            }
        }

        // Get a list of card IDs based on the selected rarity
        List<int> cardIDs = GetCardIDsByRarity(rarity);

        // Fallback to common cards if no cards are found for the selected rarity
        if (cardIDs.Count == 0)
        {
            Debug.LogError($"No cards found for rarity {rarity}. Using a common card.");
            cardIDs = CardLibrary.CardsByRarity[CardRarityType.Common];
        }

        // Select a random card ID from the list
        int randomIndex = Random.Range(0, cardIDs.Count);
        int cardID = cardIDs[randomIndex];

        return new CardReward(cardID);
    }

    /// <summary>
    /// Retrieves a list of card IDs based on the given rarity type.
    /// </summary>
    /// <param name="rarity">The rarity type for which to retrieve card IDs.</param>
    /// <returns>A list of card IDs for the specified rarity.</returns>
    private List<int> GetCardIDsByRarity(CardRarityType rarity)
    {
        if (!CardLibrary.CardsByRarity.TryGetValue(rarity, out var cardIDs))
        {
            Debug.LogWarning($"No cards found for rarity {rarity}. Using a common card.");
            cardIDs = CardLibrary.CardsByRarity[CardRarityType.Common];
        }
        return cardIDs;
    }

    #endregion
}
