using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class PlayerManager : SerializedMonoBehaviour
{
    // Singleton Instance
    public static PlayerManager Instance { get; private set; }
    public DeckSystem Decks;

    [SerializeField] private Dictionary<int, int> _startingDeck = new();

    [Header("Initial Player Stats")]
    [SerializeField] private int initialHealth = 100; // Default health value
    [SerializeField] private int initialGold = 50;    // Default gold value

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI healthText; // Text element to show health
    [SerializeField] private TextMeshProUGUI goldText;   // Text element to show gold
    // Public Properties for Current Stats
    public int CurrentHealth => PlayerData.GetHealth();
    public int CurrentGold => PlayerData.GetGold();

    //flags
    public bool hasUnlockedStoryBranch = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple PlayerManager instances found! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Decks = new DeckSystem(_startingDeck);

        // Initialize PlayerData with the specified values
        PlayerData.SetHealth(initialHealth);
        PlayerData.SetGold(initialGold);

        // Update the UI with the current values
        UpdateUI();

        Debug.Log($"Player initialized: Health = {CurrentHealth}, Gold = {CurrentGold}");
    }

    /// <summary>
    /// Updates the health and gold UI text.
    /// </summary>
    private void UpdateUI()
    {
        healthText.text = $"Health: {CurrentHealth}";
        goldText.text = $"Gold: {CurrentGold}";
    }

    /// <summary>
    /// Decreases the player's health and updates the UI.
    /// </summary>
    public void DecreaseHealth(int amount)
    {
        PlayerData.DecreaseHealth(amount);
        UpdateUI();
    }

    /// <summary>
    /// Increases the player's health and updates the UI.
    /// </summary>
    public void IncreaseHealth(int amount)
    {
        PlayerData.IncreaseHealth(amount);
        UpdateUI();
    }

    /// <summary>
    /// Decreases the player's gold and updates the UI.
    /// </summary>
    public void DecreaseGold(int amount)
    {
        PlayerData.DecreaseGold(amount);
        UpdateUI();
    }

    /// <summary>
    /// Increases the player's gold and updates the UI.
    /// </summary>
    public void IncreaseGold(int amount)
    {
        PlayerData.IncreaseGold(amount);
        UpdateUI();
    }

}
