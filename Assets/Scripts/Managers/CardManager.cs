using System.Collections.Generic;
using Sirenix.OdinInspector;
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

        for (int i = 0; i < 3; i++)
        {
            RarityType rarity = GetRandomRarity();

            // Check if the rarity key exists in the dictionary
            if (!_rarityCards.ContainsKey(rarity) || _rarityCards[rarity].Count == 0)
            {
                // If no cards are available for this rarity, handle the error
                Debug.LogWarning($"No cards available for rarity {rarity}. Falling back to common cards.");

                // Fallback: You can choose to default to common rarity or some other behavior
                rarity = RarityType.Common;

                // Ensure fallback rarity exists and has cards
                if (!_rarityCards.ContainsKey(rarity) || _rarityCards[rarity].Count == 0)
                {
                    // If even the fallback has no cards, handle the case (e.g., skip this reward)
                    Debug.LogError($"No cards available for fallback rarity {rarity}. Skipping reward generation.");
                    continue;
                }
            }
            Debug.Log($"Reward {i} has {rarity}");
            // Now that we have a valid rarity, select a random card from it
            int randomIndex = _random.Next(0, _rarityCards[rarity].Count);
            int cardID = _rarityCards[rarity][randomIndex];
            CardScriptable cardScriptable = GetCardScriptableByID(cardID);

            // Add the reward
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
            Debug.LogError($"Card with ID {cardID} not found.");
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
}