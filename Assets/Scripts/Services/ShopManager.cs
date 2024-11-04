using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class ShopManager : SerializedMonoBehaviour
{
    // // Singleton instance
    public static ShopManager Instance { get; private set; }
    // public int Gold = 100;
    // [SerializeField] private GameObject shopItemsContainer; // GridLayoutGroup container
    // [SerializeField] private TMP_Text playerGoldText;        // UI to display player's gold
    // public Dictionary<CardRarityType, int> rarityPrices = new()  // Rarity prices stored here
    // {
    //     { CardRarityType.Common, 10 },
    //     { CardRarityType.Rare, 25 },
    //     { CardRarityType.Epic, 50 },
    //     { CardRarityType.Mythic, 100 }
    // };

    // public Dictionary<int, Dictionary<CardRarityType, int>> _shopItemsLevelRarityWeights = new()
    // {
    //     { 1, new Dictionary<CardRarityType, int>
    //         {
    //             { CardRarityType.Common, 70 },
    //             { CardRarityType.Rare, 25 },
    //             { CardRarityType.Epic, 4 },
    //             { CardRarityType.Mythic, 1 }
    //         }
    //     },
    //     { 2, new Dictionary<CardRarityType, int>
    //         {
    //             { CardRarityType.Common, 60 },
    //             { CardRarityType.Rare, 30 },
    //             { CardRarityType.Epic, 8 },
    //             { CardRarityType.Mythic, 2 }
    //         }
    //     },
    //     { 3, new Dictionary<CardRarityType, int>
    //         {
    //             { CardRarityType.Common, 50 },
    //             { CardRarityType.Rare, 30 },
    //             { CardRarityType.Epic, 15 },
    //             { CardRarityType.Mythic, 5 }
    //         }
    //     },
    //     { 4, new Dictionary<CardRarityType, int>
    //         {
    //             { CardRarityType.Common, 40 },
    //             { CardRarityType.Rare, 35 },
    //             { CardRarityType.Epic, 20 },
    //             { CardRarityType.Mythic, 5 }
    //         }
    //     },
    //     { 5, new Dictionary<CardRarityType, int>
    //         {
    //             { CardRarityType.Common, 30 },
    //             { CardRarityType.Rare, 35 },
    //             { CardRarityType.Epic, 25 },
    //             { CardRarityType.Mythic, 10 }
    //         }
    //     }
    // };

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
            return;
        }
    }

    // private void SetupShop(int level)
    // {
    //     // Generate shop cards based on the given level
    //     List<int> generatedCardIDs = GenerateShopCardIDs(level);

    //     // Initialize the shop with these cards
    //     InitializeAndInstantiateShopCards(generatedCardIDs, shopItemsContainer.transform);

    //     // Update the player's gold UI
    //     UpdatePlayerGoldUI();
    // }

    // private void InitializeAndInstantiateShopCards(List<int> cardIDs, Transform parent)
    // {
    //     // Clear the existing cards in the shop container
    //     foreach (Transform child in parent)
    //     {
    //         Destroy(child.gameObject);
    //     }

    //     // Initialize and instantiate each card
    //     foreach (int cardID in cardIDs)
    //     {
    //         InstantiateShopCard(cardID, parent);
    //     }
    // }

    // private void InstantiateShopCard(int cardID, Transform parent)
    // {
    //     // Instantiate the card using CardManager
    //     Card cardInstance = CardSystem.Instance.InstantiateCard(cardID, parent);
    //     cardInstance.isInShop = true;
    //     // Set the card's price based on its rarity
    //     int price = GetPriceByRarity(cardInstance.Rarity);
    //     cardInstance.Price = price;
    //     cardInstance.UpdatePriceValue();
    // }

    // private List<int> GenerateShopCardIDs(int level)
    // {
    //     // Generate 9 card IDs using the utility method
    //     return RandomRarityUtil.GenerateCardIDsByRarity(
    //         CardSystem.Instance.GetRarityCards(), // Available cards by rarity
    //         _shopItemsLevelRarityWeights, // Rarity weights per level
    //         level, // Current level
    //         9, // Number of cards to generate
    //         true // Allow duplicates
    //     );
    // }

    // public int GetPriceByRarity(CardRarityType rarity)
    // {
    //     return rarityPrices.TryGetValue(rarity, out int price) ? price : 0;
    // }

    // public void SpendGold(int amount)
    // {
    //     Gold -= amount;
    //     UpdatePlayerGoldUI();
    // }

    // public void UpdatePlayerGoldUI()
    // {
    //     playerGoldText.text = Gold.ToString();
    // }

    // //Open or close the shop canvas
    // public void InitializeShop()
    // {
    //     UpdatePlayerGoldUI();
    //     SetupShop(GameManager.Instance.CurrentLevel);
    // }
}
