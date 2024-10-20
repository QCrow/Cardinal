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
    private List<Card> _shopDeck = new();
    private List<Card> _availableCards = new();
    public Transform Graveyard;

    private Dictionary<RarityType, int> _rarityWeights = new()
    {
        { RarityType.Common, 70 },
        { RarityType.Rare, 15 },
        { RarityType.Epic, 4 },
        { RarityType.Mythic, 1 }
    };

    private Dictionary<RarityType, List<int>> _rarityCards = new();

    private RarityType GetRandomRarity()
    {
        int totalWeight = 0;
        foreach (KeyValuePair<RarityType, int> rarity in _rarityWeights)
        {
            totalWeight += rarity.Value;
        }

        int randomValue = _random.Next(0, totalWeight);
        int weightSum = 0;

        foreach (KeyValuePair<RarityType, int> rarity in _rarityWeights)
        {
            weightSum += rarity.Value;
            if (randomValue < weightSum)
            {
                return rarity.Key;
            }
        }

        return RarityType.Common;
    }

    public List<Reward> GenerateRewardChoices()
    {
        List<Reward> rewards = new();
        HashSet<int> selectedCardIDs = new();  // Use a HashSet to track selected card IDs
        Dictionary<RarityType, List<int>> remainingCards = new();

        // Copy available cards from each rarity to a new dictionary to track remaining cards
        foreach (var rarity in _rarityCards)
        {
            remainingCards[rarity.Key] = new List<int>(rarity.Value);
        }

        // Generate 3 rewards
        for (int i = 0; i < 3; i++)
        {
            RarityType rarity = GetRandomRarity();

            // Keep looping until we find a valid rarity with available cards
            while (!remainingCards.ContainsKey(rarity) || remainingCards[rarity].Count == 0)
            {
                Debug.LogWarning($"No cards left for rarity {rarity}. Trying a different rarity.");
                rarity = GetRandomRarity();

                // If no rarities have cards left, break out of the loop
                if (remainingCards.All(r => r.Value.Count == 0))
                {
                    Debug.LogError("No cards left in any rarity. Stopping reward generation.");
                    return rewards;
                }
            }

            // Select a random card from the remaining cards of the selected rarity
            int randomIndex = _random.Next(0, remainingCards[rarity].Count);
            int cardID = remainingCards[rarity][randomIndex];
            remainingCards[rarity].RemoveAt(randomIndex);  // Remove selected card from the pool

            // Ensure no duplicates
            selectedCardIDs.Add(cardID);

            CardScriptable cardScriptable = GetCardScriptableByID(cardID);
            rewards.Add(new Reward(cardID, cardScriptable.Name, cardScriptable.Description));
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

    public void InitializeAndInstantiateShopCards(Dictionary<int, int> startingShopDeck, Transform parent)
    {
        _shopDeck.Clear();  // Clear any existing cards

        int totalCards = 0;  // Track how many cards are added

        // Populate the shop deck with up to 9 cards
        foreach (KeyValuePair<int, int> card in startingShopDeck)
        {
            for (int i = 0; i < card.Value; i++)
            {
                if (totalCards >= 9)  // Ensure only 9 cards are added
                    break;

                Card cardInstance = InstantiateCard(card.Key);  // Create card instance
                _shopDeck.Add(cardInstance);  // Add to shop deck
                totalCards++;  // Increment the counter

                // Instantiate the card in the parent container
                var newcard = InstantiateCard(cardInstance.ID, parent);

                int price = ShopManager.Instance.GetPriceByRarity(cardInstance.Rarity);
                newcard.Price = price;
                newcard.UpdatePriceValue();
            }

            if (totalCards >= 9)  // Stop if we have enough cards
                break;
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

    //TODO: Implement this method
    public void TransformCard(Card card, int cardID)
    {
        throw new NotImplementedException();
    }
}