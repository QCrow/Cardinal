using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CardManager : SerializedMonoBehaviour
{
    #region Singleton
    private static CardManager _instance;
    public static CardManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            LoadCards();
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private readonly System.Random _random = new();
    [SerializeField] private Dictionary<int, int> _startingDeck = new();
    private List<Card> _deck = new();
    private List<Card> _availableCards = new();
    public Transform Graveyard;

    // Define a shared dictionary of rarity weights for all levels
    [SerializeField, ShowInInspector]
    private Dictionary<RarityType, int> _sharedRarityWeights = new()
    {
        { RarityType.Common, 70 },
        { RarityType.Rare, 15 },
        { RarityType.Epic, 4 },
        { RarityType.Mythic, 1 }
    };

    [SerializeField, ShowInInspector]
    private Dictionary<int, Dictionary<RarityType, int>> _rewardLevelRarityWeights = new()
    {
        { 1, new Dictionary<RarityType, int>
            {
                { RarityType.Common, 70 },
                { RarityType.Rare, 15 },
                { RarityType.Epic, 4 },
                { RarityType.Mythic, 1 }
            }
        },
        { 2, new Dictionary<RarityType, int>
            {
                { RarityType.Common, 60 },
                { RarityType.Rare, 25 },
                { RarityType.Epic, 10 },
                { RarityType.Mythic, 5 }
            }
        }
    };

    private Dictionary<RarityType, List<int>> _rarityCards = new();

    public Dictionary<RarityType, List<int>> GetRarityCards() { return _rarityCards; }

    //if we want same rarity weights across different levels
    private void InitializeRewardLevelRarityWeights(int maxLevel)
    {
        for (int level = 1; level <= maxLevel; level++)
        {
            _rewardLevelRarityWeights[level] = _sharedRarityWeights;
        }
    }

    public List<Reward> GenerateRewardChoices()
    {
        // Generate 3 reward card IDs using the utility method
        List<int> rewardCardIDs = RandomRarityUtil.GenerateCardIDsByRarity(
            _rarityCards,              // Available cards grouped by rarity
            _rewardLevelRarityWeights,       // Rarity weights per level
            GameManager.Instance.CurrentLevel,                     // Current game level
            3,                         // Number of reward cards to generate
            false                      // No duplicate rewards allowed
        );

        
        // Create Reward objects from the generated card IDs
        List<Reward> rewards = new();
        foreach (int cardID in rewardCardIDs)
        {
            CardScriptable cardScriptable = GetCardScriptableByID(cardID);
            if (cardScriptable != null)
            {
                rewards.Add(new Reward(cardID, cardScriptable.Name, cardScriptable.Description));
            }
        }

        return rewards;
    }



    private void LoadCards()
    {
        _rarityCards.Clear();

        foreach (CardScriptable card in Resources.LoadAll<CardScriptable>("Cards"))
        {
            if (!_rarityCards.ContainsKey(card.Rarity))
            {
                _rarityCards.Add(card.Rarity, new List<int>());
            }

            _rarityCards[card.Rarity].Add(card.ID);
        }
    }

    [SerializeField] private GameObject _cardPrefab;

    private void LogDeck()
    {
        string deck = "Deck: ";
        foreach (Card card in _deck)
        {
            deck += card.ID + " ";
        }
        Debug.Log(deck);
    }

    public void InitializeDeck()
    {
        foreach (KeyValuePair<int, int> card in _startingDeck)
        {
            for (int i = 0; i < card.Value; i++)
            {
                Card cardInstance = InstantiateCard(card.Key);
                _deck.Add(cardInstance);
            }
        }
    }

    public void Reshuffle()
    {

        _availableCards.Clear();
        _availableCards.AddRange(_deck);

        for (int i = _availableCards.Count - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (_availableCards[j], _availableCards[i]) = (_availableCards[i], _availableCards[j]);
        }
    }

    public Card DrawCard()
    {
        if (_availableCards.Count == 0)
        {
            return null;
        }

        Card card = _availableCards[0];
        _availableCards.RemoveAt(0);

        return card;
    }

    public void AddCard(int cardID)
    {
        Card card = InstantiateCard(cardID);
        _deck.Add(card);
        _availableCards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        _deck.Remove(card);
        _availableCards.Remove(card);
    }

    public void ClearDeck()
    {
        _deck.Clear();
    }

    public CardScriptable GetCardScriptableByID(int cardID)
    {
        return Resources.Load<CardScriptable>($"Cards/Card_{cardID}");
    }

    public Card InstantiateCard(int cardID)
    {
        CardScriptable scriptable = GetCardScriptableByID(cardID);
        if (scriptable == null)
        {
            UnityEngine.Debug.LogError($"Card with ID {cardID} not found.");
            return null;
        }

        GameObject cardObject = Instantiate(_cardPrefab, Graveyard);
        Card card = CardFactory.CreateCard(cardObject, scriptable);

        return card;
    }

    public Card InstantiateCard(int cardID, Transform parent)
    {
        Card card = InstantiateCard(cardID);
        card.transform.SetParent(parent, false);

        return card;
    }


    public int InstantiateDeckToParent(Transform parent)
    {
        // Get the grouped deck (one entry per unique card ID with its count)
        Dictionary<int, int> groupedDeck = GetGroupedDeck();

        // Iterate through the grouped deck to instantiate cards
        foreach (var kvp in groupedDeck)
        {
            int cardID = kvp.Key;
            int quantity = kvp.Value;

            // Instantiate a single card for each unique card ID
            Card newCard = InstantiateCard(cardID, parent);

            // Disable cycling if applicable
            newCard.DisableCycleContainer();

            // Find the TMP_Text for amount on the card and set the quantity
            var amountText = newCard._amountInDeck;
            if (amountText != null)
            {
                amountText.text = quantity.ToString();
            }
        }

        // Return the total number of unique cards (number of keys in the dictionary)
        return groupedDeck.Count;
    }

    public Dictionary<int, int> GetGroupedDeck()
    {
        Dictionary<int, int> groupedDeck = new();

        foreach (var card in _deck)
        {
            if (groupedDeck.ContainsKey(card.ID))
            {
                groupedDeck[card.ID]++;
            }
            else
            {
                groupedDeck[card.ID] = 1;
            }
        }

        return groupedDeck;
    }

    public void TransformCard(Card card, int cardID)
    {
        throw new NotImplementedException();
    }
}