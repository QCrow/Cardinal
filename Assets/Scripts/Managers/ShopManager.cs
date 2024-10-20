using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class ShopManager : MonoBehaviour
{
    // Singleton instance
    public static ShopManager Instance { get; private set; }
    public int Gold = 100;
    [SerializeField] private GameObject shopItemsContainer; // GridLayoutGroup container
    [SerializeField] private TMP_Text playerGoldText;        // UI to display player's gold
    [ShowInInspector, SerializeField]
    private Dictionary<int, int> _startingShopDeck;
    [ShowInInspector, SerializeField]
    public Dictionary<RarityType, int> rarityPrices = new()  // Rarity prices stored here
    {
        { RarityType.Common, 10 },
        { RarityType.Rare, 25 },
        { RarityType.Epic, 50 },
        { RarityType.Mythic, 100 }
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
        _startingShopDeck = new Dictionary<int, int>()
        {
            {1,1},
            {2,1},
            {3,1},
            {4,1},
            {5,1},
            {6,1},
            {7,1},
            {8,1},
            {9,1},
        };
        UpdatePlayerGoldUI();
    }

    public void InitializeShop()
    {
        CardManager.Instance.InitializeAndInstantiateShopCards(_startingShopDeck, shopItemsContainer.transform);
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
}
