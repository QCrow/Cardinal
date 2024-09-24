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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private System.Random _random = new();
    [SerializeField] private Dictionary<int, int> _startingDeck = new();
    private List<Card> _deck = new();
    private List<Card> _availableCards = new();
    public Transform Graveyard;

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
        LogDeck();
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

        GameObject cardObject = Instantiate(_cardPrefab);
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