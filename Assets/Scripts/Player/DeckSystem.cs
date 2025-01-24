using UnityEngine;
using System.Collections.Generic;

public class DeckSystem
{
    // The player's inventory with all obtained cards, that carries over between battles and chapters
    private List<CardInstance> _inventory = new();
    // The player's selected deck
    private List<CardInstance> _selectedDeck = new();
    // The deck temporarily used during a battle, with temporarily added/removed/transformed cards
    private List<CardInstance> _battleDeck = new();
    // The deck used for drawing cards during a battle, which is refilled every turn
    private List<CardInstance> _drawPool = new();

    private int _cardShuffleSeed;

    public DeckSystem(List<CardInstance> startingDeck)
    {
        _selectedDeck = startingDeck;

        _cardShuffleSeed = GameManager.Instance.Seed * 37 + 11;
    }

    public DeckSystem(Dictionary<int, int> startingDeck)
    {
        foreach (KeyValuePair<int, int> cardEntry in startingDeck)
        {
            for (int i = 0; i < cardEntry.Value; i++)
            {
                AddCard(cardEntry.Key, true);
            }
        }
    }

    public void SetSeed(int seed)
    {
        _cardShuffleSeed = seed;
    }

    public bool HasCard(int cardID)
    {
        return _selectedDeck.Exists(card => card.Template.ID == cardID);
    }

    public void InitializeBeforeBattle()
    {
        // Copy the player's deck to the battle deck
        _battleDeck = new List<CardInstance>(_selectedDeck);

        // Shuffle the draw pool
        ShuffleDrawPool();
    }

    public void ResetAfterBattle()
    {
        // Reset all card modifiers to their default state in the deck
        foreach (CardInstance card in _selectedDeck)
        {
            card.ResetCardModifierState(ModifierPersistenceType.Battle);
        }
    }

    public void ShuffleDrawPool()
    {
        _cardShuffleSeed = GameManager.Instance.GetDerivedSeedWithPosition(_cardShuffleSeed, 15485863, 99991);
        Random.InitState(_cardShuffleSeed);
        // Copy the battle deck to the draw pool
        _drawPool = new List<CardInstance>(_battleDeck);

        //System.Random random = new();

        // Fisher-Yates shuffle algorithm
        for (int i = _drawPool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (_drawPool[i], _drawPool[j]) = (_drawPool[j], _drawPool[i]);
        }
    }

    public CardInstance DrawCard()
    {
        if (_drawPool.Count == 0) return null;

        CardInstance card = _drawPool[0];
        _drawPool.RemoveAt(0);
        return card;
    }

    public CardInstance AddCard(int cardID, bool isPermanent = false)
    {
        CardInstance card = CardSystem.Instance.BuildCardInstance(cardID, CardSystem.Instance.GraveyardTransform);
        AddCard(card, isPermanent);
        return card;
    }

    public CardInstance AddCard(CardInstance card, bool isPermanent = false)
    {
        if (isPermanent)
        {
            _selectedDeck.Add(card);
        }
        _battleDeck.Add(card);
        return card;
    }

    public void RemoveCard(CardInstance card, bool isPermanent = false)
    {
        if (isPermanent)
        {
            _selectedDeck.Remove(card);
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

    public CardInstance TransformCard(CardInstance card, int cardID, bool isPermanent = false)
    {
        // Create the new card with the specified ID
        CardInstance newCard = CardSystem.Instance.BuildCardInstance(cardID);

        // Check if the transformed card is already on the board
        if (card.CurrentSlot != null)
        {
            Slot slot = card.CurrentSlot;

            // Bind the new card to the same slot
            newCard.BindToSlot(slot);
        }

        Board.Instance.UpdateDeployedCards();
        AddCard(newCard, isPermanent);
        RemoveCard(card, isPermanent);
        return newCard;
    }
}