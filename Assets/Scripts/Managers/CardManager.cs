using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Fields

    private readonly System.Random _random = new();
    [SerializeField] private GameObject _cardPrefab;

    [SerializeField] private Dictionary<int, int> _startingDeck = new();

    [SerializeField] private List<Card> _permanentDeck = new();
    private List<Card> _temporaryDeck = new();
    private List<Card> _availableCards = new();
    public Transform Graveyard;

    [SerializeField, ShowInInspector]
    private Dictionary<RarityType, int> _sharedRarityWeights = new()
    {
        { RarityType.Common, 70 },
        { RarityType.Rare, 15 },
        { RarityType.Epic, 4 },
        { RarityType.Mythic, 1 }
    };

    [SerializeField, ShowInInspector]
    private Dictionary<int, Dictionary<RarityType, int>> _rewardLevelRarityWeights = new();

    private Dictionary<RarityType, List<int>> _rarityCards = new();

    #endregion

    #region Initialization

    private void LoadCards()
    {
        _rarityCards.Clear();

        foreach (CardScriptable card in Resources.LoadAll<CardScriptable>("Cards"))
        {
            if (!_rarityCards.ContainsKey(card.Rarity))
            {
                _rarityCards[card.Rarity] = new List<int>();
            }
            _rarityCards[card.Rarity].Add(card.ID);
        }

        foreach (var card in _startingDeck)
        {
            for (int i = 0; i < card.Value; i++)
            {
                _permanentDeck.Add(InstantiateCard(card.Key));
            }
        }
    }

    private void InitializeDefaultRewardLevelRarityWeights(int maxLevel)
    {
        for (int level = 1; level <= maxLevel; level++)
        {
            _rewardLevelRarityWeights[level] = _sharedRarityWeights;
        }
    }

    public void ResetDecks()
    {
        _temporaryDeck.Clear();
        _availableCards.Clear();

        _temporaryDeck.AddRange(_permanentDeck);
        foreach (Card card in _temporaryDeck)
        {
            card.ResetAllState();
        }
        Reshuffle();
    }
    #endregion

    #region Deck Operations
    public void AddCardPermanently(int cardID)
    {
        Card card = InstantiateCard(cardID);
        _permanentDeck.Add(card);
    }

    public Card AddCardTemporarily(int cardID)
    {
        Card card = InstantiateCard(cardID);
        _temporaryDeck.Add(card);
        _availableCards.Add(card);
        return card;
    }

    public void DestroyCard(Card card)
    {
        _permanentDeck.Remove(card);
    }

    public void RemoveCard(Card card)
    {
        _temporaryDeck.Remove(card);
        _availableCards.Remove(card);
        card.transform.SetParent(Graveyard, false);
        card.transform.localPosition = Vector3.zero;
    }

    public void Reshuffle()
    {
        _availableCards.Clear();
        _availableCards.AddRange(_temporaryDeck);

        for (int i = _availableCards.Count - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (_availableCards[i], _availableCards[j]) = (_availableCards[j], _availableCards[i]);
        }
    }

    public Card DrawCard()
    {
        if (_availableCards.Count == 0) return null;

        Card card = _availableCards[0];
        _availableCards.RemoveAt(0);
        return card;
    }

    public Dictionary<int, int> GetGroupedDeck()
    {
        return _permanentDeck
            .GroupBy(card => card.ID)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public int InstantiateDeckToParent(Transform parent)
    {
        var groupedDeck = GetGroupedDeck();
        foreach (var kvp in groupedDeck)
        {
            var newCard = InstantiateCard(kvp.Key, parent);
            newCard.DisableCycleContainer();

            if (newCard._amountInDeck != null)
            {
                newCard._amountInDeck.text = kvp.Value.ToString();
            }
        }
        return groupedDeck.Count;
    }

    #endregion

    #region Reward Generation

    public List<Reward> GenerateRewardChoices()
    {
        InitializeDefaultRewardLevelRarityWeights(5);

        List<int> rewardCardIDs = RandomRarityUtil.GenerateCardIDsByRarity(
            _rarityCards,
            _rewardLevelRarityWeights,
            GameManager.Instance.CurrentLevel,
            3,
            false
        );

        return rewardCardIDs
            .Select(cardID => GetCardScriptableByID(cardID))
            .Where(cardScriptable => cardScriptable != null)
            .Select(cs => new Reward(cs.ID, cs.Name, cs.Description, cs.BaseAttack))
            .ToList();
    }

    #endregion

    #region Card Instantiation

    public Card InstantiateCard(int cardID)
    {
        CardScriptable scriptable = GetCardScriptableByID(cardID);
        if (scriptable == null)
        {
            Debug.LogError($"Card with ID {cardID} not found.");
            return null;
        }

        var cardObject = Instantiate(_cardPrefab, Graveyard);
        return CardFactory.CreateCard(cardObject, scriptable);
    }

    public Card InstantiateCard(int cardID, Transform parent)
    {
        Card card = InstantiateCard(cardID);
        if (card != null) card.transform.SetParent(parent, false);
        return card;
    }

    public CardScriptable GetCardScriptableByID(int cardID)
    {
        return Resources.Load<CardScriptable>($"Cards/Card_{cardID}");
    }

    #endregion

    #region TODO

    public Card TransformCardTemporarily(Card card, int cardID)
    {
        Transform parent = card.transform.parent;
        Card newCard = InstantiateCard(cardID, parent);

        if (card.Slot != null)
        {
            Slot slot = card.Slot;
            card.UnbindFromSlot();
            newCard.BindToSlot(slot);
        }
        Board.Instance.SyncDeployedCards();
        _temporaryDeck.Add(newCard);
        _availableCards.Add(newCard);
        _temporaryDeck.Remove(card);
        _availableCards.Remove(card);
        return newCard;
    }

    public Card TransformCardPermanently(Card card, int cardID)
    {
        Transform parent = card.transform.parent;
        Card newCard = InstantiateCard(cardID, parent);

        if (card.Slot != null)
        {
            Slot slot = card.Slot;
            card.UnbindFromSlot();
            newCard.BindToSlot(slot);
        }
        Board.Instance.SyncDeployedCards();
        _permanentDeck.Add(newCard);
        _temporaryDeck.Add(newCard);
        _availableCards.Add(newCard);
        _temporaryDeck.Remove(card);
        _permanentDeck.Remove(card);
        _availableCards.Remove(card);
        return newCard;
    }
    #endregion

    #region Accessors

    public Dictionary<RarityType, List<int>> GetRarityCards() => _rarityCards;

    #endregion
}
