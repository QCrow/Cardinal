using UnityEngine;
using System.Collections.Generic;

public class DeckManager
{
    // The player's deck, that carries over between battles and chapters
    private List<Card> _deck = new();
    // The deck temporarily used during a battle, with temporarily added/removed/transformed cards
    private List<Card> _battleDeck = new();

    // The deck used for drawing cards during a battle, which is refilled every turn
    private List<Card> _drawPool = new();

    public DeckManager(List<Card> startingDeck)
    {
        _deck = startingDeck;
    }

    public DeckManager(Dictionary<int, int> startingDeck)
    {
        foreach (KeyValuePair<int, int> cardEntry in startingDeck)
        {
            for (int i = 0; i < cardEntry.Value; i++)
            {
                AddCard(cardEntry.Key, true);
            }
        }
    }

    public bool HasCard(int cardID)
    {
        return _deck.Exists(card => card.ID == cardID);
    }

    public void InitializeBeforeBattle()
    {
        // Copy the player's deck to the battle deck
        _battleDeck = new List<Card>(_deck);

        // Shuffle the draw pool
        ShuffleDrawPool();
    }

    public void ResetAfterBattle()
    {
        // Reset all card modifiers to their default state in the deck
        foreach (Card card in _deck)
        {
            card.ResetCardModifierState(ModifierPersistenceType.Battle);
        }
    }

    public void ShuffleDrawPool()
    {
        // Copy the battle deck to the draw pool
        _drawPool = new List<Card>(_battleDeck);

        System.Random random = new();

        // Fisher-Yates shuffle algorithm
        for (int i = _drawPool.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (_drawPool[i], _drawPool[j]) = (_drawPool[j], _drawPool[i]);
        }
    }

    public Card DrawCard()
    {
        if (_drawPool.Count == 0) return null;

        Card card = _drawPool[0];
        _drawPool.RemoveAt(0);
        return card;
    }

    public Card AddCard(int cardID, bool isPermanent = false)
    {
        Card card = CardSystem.Instance.CardRepository.InstantiateCard(cardID);
        AddCard(card, isPermanent);
        return card;
    }

    public Card AddCard(Card card, bool isPermanent = false)
    {
        if (isPermanent)
        {
            _deck.Add(card);
        }
        _battleDeck.Add(card);
        return card;
    }

    public void RemoveCard(Card card, bool isPermanent = false)
    {
        if (isPermanent)
        {
            _deck.Remove(card);
            _battleDeck.Remove(card);
            card.UnbindFromSlot();
            Object.Destroy(card.gameObject);
        }
        else
        {
            _battleDeck.Remove(card);
            _drawPool.Remove(card);
            card.UnbindFromSlot();
            card.MoveToGraveyard();
        }
    }

    public Card TransformCard(Card card, int cardID, bool isPermanent = false)
    {
        // Create the new card with the specified ID
        Card newCard = CardSystem.Instance.CardRepository.InstantiateCard(cardID);

        // Check if the transformed card is already on the board
        if (card.CurrentSlot != null)
        {
            Slot slot = card.CurrentSlot;

            // Bind the new card to the same slot
            newCard.BindToSlot(slot);
        }

        Board.Instance.UpdateDeployedCards();
        CardSystem.Instance.DeckManager.AddCard(newCard, isPermanent);
        CardSystem.Instance.DeckManager.RemoveCard(card, isPermanent);
        return newCard;
    }
}