using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles loading and instantiating cards from resources.
/// </summary>
public class CardRepository
{
    // Prefab used to instantiate card objects
    private GameObject _cardPrefab;

    // Dictionary storing card IDs grouped by rarity type
    private Dictionary<CardRarityType, List<int>> _cardsByRarity = new();
    public Dictionary<CardRarityType, List<int>> CardsByRarity => _cardsByRarity;

    // List of cards with the "Unique" rarity type
    public HashSet<int> UniqueCardPool = new();

    /// <summary>
    /// Initializes a new instance of the CardLoader class.
    /// </summary>
    /// <param name="cardPrefab">The prefab used to instantiate card objects.</param>
    public CardRepository(GameObject cardPrefab)
    {
        // Ensure the card prefab is not null
        if (cardPrefab == null)
        {
            Debug.LogError("Card prefab is not set. Please provide a valid card prefab.");
            return;
        }

        _cardPrefab = cardPrefab;
        LoadCards();
    }

    /// <summary>
    /// Loads all card data from the "Cards" resource folder.
    /// </summary>
    public void LoadCards()
    {
        // Load all CardScriptable objects from the "Cards" resource folder
        foreach (CardScriptable card in Resources.LoadAll<CardScriptable>("Cards"))
        {
            // Group card IDs by rarity
            if (!_cardsByRarity.ContainsKey(card.Rarity))
            {
                _cardsByRarity[card.Rarity] = new List<int>();
            }
            _cardsByRarity[card.Rarity].Add(card.ID);
        }

        // Create a unique card pool for random card generation
        if (_cardsByRarity.ContainsKey(CardRarityType.Unique))
        {
            UniqueCardPool.UnionWith(_cardsByRarity[CardRarityType.Unique]);
        }
    }

    #region Card Retrieval

    /// <summary>
    /// Instantiates a card with the specified ID and places it in the graveyard by default.
    /// </summary>
    /// <param name="id">The ID of the card to instantiate.</param>
    /// <returns>The instantiated Card object.</returns>
    public Card InstantiateCard(int id)
    {
        return InstantiateCard(id, CardSystem.Instance.Graveyard);
    }

    /// <summary>
    /// Instantiates a card with the specified ID and parent transform.
    /// </summary>
    /// <param name="id">The ID of the card to instantiate.</param>
    /// <param name="parent">The parent transform to attach the instantiated card to.</param>
    /// <returns>The instantiated Card object, or null if the card could not be created.</returns>
    public Card InstantiateCard(int id, Transform parent)
    {
        // Retrieve the CardScriptable object using the provided ID
        CardScriptable cardScriptable = GetCardScriptableByID(id);
        if (cardScriptable == null)
        {
            Debug.LogError($"Card with ID {id} not found. Please check your resources.");
            return null;
        }

        // Ensure the card prefab is valid
        if (_cardPrefab == null)
        {
            Debug.LogError("Card prefab is not set. Cannot instantiate card.");
            return null;
        }

        // Instantiate the card prefab and set its parent transform
        GameObject cardObject = Object.Instantiate(_cardPrefab, parent);

        // Ensure the instantiated object has a Card component and reset its transform
        if (cardObject.TryGetComponent<Card>(out Card card))
        {
            card.ResetTransform();
        }
        else
        {
            Debug.LogError("Instantiated object does not have a Card component.");
            return null;
        }

        // Use the CardFactory to finalize card creation with effects
        return CardFactory.CreateCard(cardObject, cardScriptable);
    }

    /// <summary>
    /// Retrieves a CardScriptable object by ID from the "Cards" resource folder.
    /// </summary>
    /// <param name="id">The ID of the card to retrieve.</param>
    /// <returns>The CardScriptable object with the specified ID, or null if not found.</returns>
    public CardScriptable GetCardScriptableByID(int id)
    {
        return Resources.Load<CardScriptable>($"Cards/Card_{id}");
    }
    #endregion
}
