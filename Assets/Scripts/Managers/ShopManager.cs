using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class ShopManager : MonoBehaviour
{
    // Singleton instance
    public static ShopManager Instance { get; private set; }
    public int Gold = 100;
    [SerializeField] private GameObject shopCanvas;  // Canvas to toggle
    [SerializeField] private GameObject shopItemsContainer; // GridLayoutGroup container
    [SerializeField] private TMP_Text playerGoldText;        // UI to display player's gold
    [ShowInInspector, SerializeField]
    public Dictionary<RarityType, int> rarityPrices = new()  // Rarity prices stored here
    {
        { RarityType.Common, 10 },
        { RarityType.Rare, 25 },
        { RarityType.Epic, 50 },
        { RarityType.Mythic, 100 }
    };

    [SerializeField, ShowInInspector]
    private Dictionary<int, Dictionary<RarityType, int>> _shopItemsLevelRarityWeights = new()
    {
        { 1, new Dictionary<RarityType, int>
            {
                { RarityType.Common, 70 },
                { RarityType.Rare, 25 },
                { RarityType.Epic, 4 },
                { RarityType.Mythic, 1 }
            }
        },
        { 2, new Dictionary<RarityType, int>
            {
                { RarityType.Common, 60 },
                { RarityType.Rare, 30 },
                { RarityType.Epic, 8 },
                { RarityType.Mythic, 2 }
            }
        },
        { 3, new Dictionary<RarityType, int>
            {
                { RarityType.Common, 50 },
                { RarityType.Rare, 30 },
                { RarityType.Epic, 15 },
                { RarityType.Mythic, 5 }
            }
        },
        { 4, new Dictionary<RarityType, int>
            {
                { RarityType.Common, 40 },
                { RarityType.Rare, 35 },
                { RarityType.Epic, 20 },
                { RarityType.Mythic, 5 }
            }
        },
        { 5, new Dictionary<RarityType, int>
            {
                { RarityType.Common, 30 },
                { RarityType.Rare, 35 },
                { RarityType.Epic, 25 },
                { RarityType.Mythic, 10 }
            }
        }
    };

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

    private void Start()
    {
        shopCanvas.SetActive(false);  // Ensure the shop is hidden by default
    }

    private void SetupShop(int level)
    {
        // Generate shop cards based on the given level
        List<int> generatedCardIDs = GenerateShopCardIDs(level);

        // Initialize the shop with these cards
        InitializeAndInstantiateShopCards(generatedCardIDs, shopItemsContainer.transform);

        // Update the player's gold UI
        UpdatePlayerGoldUI();
    }

    private void InitializeAndInstantiateShopCards(List<int> cardIDs, Transform parent)
    {
        // Clear the existing cards in the shop container
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        // Initialize and instantiate each card
        foreach (int cardID in cardIDs)
        {
            InstantiateShopCard(cardID, parent);
        }
    }

    private void InstantiateShopCard(int cardID, Transform parent)
    {
        // Instantiate the card using CardManager
        Card cardInstance = CardManager.Instance.InstantiateCard(cardID, parent);

        // Set the card's price based on its rarity
        int price = GetPriceByRarity(cardInstance.Rarity);
        cardInstance.Price = price;
        cardInstance.UpdatePriceValue();
    }

    private List<int> GenerateShopCardIDs(int level)
    {
        // Generate 9 card IDs using the utility method
        return RandomRarityUtil.GenerateCardIDsByRarity(
            CardManager.Instance.GetRarityCards(), // Available cards by rarity
            _shopItemsLevelRarityWeights, // Rarity weights per level
            level, // Current level
            9, // Number of cards to generate
            true // Allow duplicates
        );
    }

    public int GetPriceByRarity(RarityType rarity)
    {
        return rarityPrices.TryGetValue(rarity, out int price) ? price : 0;
    }

    public void SpendGold(int amount)
    {
        Gold -= amount;
        UpdatePlayerGoldUI();
    }

    public void UpdatePlayerGoldUI()
    {
        playerGoldText.text = Gold.ToString();
    }

    //Open or close the shop canvas
    public void ToggleShopCanvas()
    {
        UpdatePlayerGoldUI();
        bool isActive = shopCanvas.activeSelf;
        shopCanvas.SetActive(!isActive);

        // If the shop canvas is now active, setup the shop
        if (shopCanvas.activeSelf)
        {
            SetupShop(GameManager.Instance.CurrentLevel);
        }
    }
}
