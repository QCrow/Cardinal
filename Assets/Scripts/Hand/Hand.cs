using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private static Hand _instance;
    public static Hand Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Hand is null! Make sure there is a Hand in the scene.");
            }
            return _instance;
        }
    }

    private List<GameObject> _cards = new();
    private List<GameObject> _containers = new();

    [SerializeField]
    private int _maxCapacity = 7;

    [SerializeField]
    private GameObject _buildingCardPrefab;
    [SerializeField]
    private GameObject _anchorContainer;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject); // Prevent duplicate instances of Hand
            return;
        }
        _instance = this;
    }

#nullable enable
    // Method to add a specified amount of cards by ID with optional terminal logging
    public bool AddCardByID(int cardID, int amount = 1, bool logToTerminal = false)
    {
        // Check if the amount is valid
        if (amount <= 0)
        {
            string warningMessage = "Amount must be greater than zero.";
            Debug.LogWarning(warningMessage);

            if (logToTerminal)
            {
                DevTerminal.Instance?.LogToTerminal(warningMessage);
            }
            return false;
        }

        for (int i = 0; i < amount; i++)
        {
            if (_cards.Count >= _maxCapacity)
            {
                string warningMessage = "Hand is at maximum capacity, cannot add more cards.";
                Debug.LogWarning(warningMessage);

                if (logToTerminal)
                {
                    DevTerminal.Instance?.LogToTerminal(warningMessage);
                }
                return false; // Stop adding if hand is full
            }

            // Get the CardData by ID from the CardManager
            CardData? cardData = CardManager.Instance.GetCardDataByID(cardID);
            if (cardData == null)
            {
                string warningMessage = $"Card with ID {cardID} could not be found.";
                Debug.LogWarning(warningMessage);

                if (logToTerminal)
                {
                    DevTerminal.Instance?.LogToTerminal(warningMessage);
                }
                return false;
            }

            GameObject container = Instantiate(_anchorContainer, gameObject.transform);
            // Instantiate the card prefab
            GameObject card = Instantiate(_buildingCardPrefab, container.transform);

            // Use CardFactory to apply data to the card
            CardFactory.CreateCard(card.GetComponent<BuildingCard>(), cardData);
            card.GetComponent<Card>().AnchorContainer = container;

            // Add the card to the hand
            _cards.Add(card);
            _containers.Add(container);

            if (logToTerminal)
            {
                DevTerminal.Instance?.LogToTerminal($"Card with ID {cardID} added to hand. (Amount: {i + 1})");
            }
        }

        Debug.Log($"Added {amount} card(s) with ID {cardID} to hand.");
        return true;
    }


    // Method to destroy a card with optional terminal logging
    public bool DestroyCard(int index, bool logToTerminal = false)
    {
        if (index < 0 || index >= _cards.Count)
        {
            string warningMessage = "Invalid index, no card removed.";
            Debug.LogWarning(warningMessage);

            if (logToTerminal)
            {
                DevTerminal.Instance?.LogToTerminal(warningMessage);
            }
            return false;
        }

        GameObject cardToRemove = _cards[index];
        _cards.RemoveAt(index);
        RemoveContainer(cardToRemove);
        cardToRemove.GetComponent<Card>().ForceRemove();

        string successMessage = $"Card at index {index} removed.";
        Debug.Log(successMessage);

        if (logToTerminal)
        {
            DevTerminal.Instance?.LogToTerminal(successMessage);
        }

        return true;
    }

    public void DestroyAllCards(bool logToTerminal = false)
    {
        // Loop through all cards in the hand and destroy them
        for (int i = _cards.Count - 1; i >= 0; i--) // Iterate backwards to avoid index issues
        {
            GameObject card = _cards[i];
            RemoveContainer(card);
            card.GetComponent<Card>().ForceRemove();
        }

        // Clear the list after destroying all cards
        _cards.Clear();

        string successMessage = "All cards have been destroyed from the hand.";
        Debug.Log(successMessage);

        if (logToTerminal)
        {
            DevTerminal.Instance?.LogToTerminal(successMessage);
        }
    }


    //Move a card from hand to other place, i.e. to the board.
    public bool RemoveCard(GameObject card)
    {
        if (!_cards.Contains(card))
        {
            Debug.LogWarning("Card not found in hand.");
            return false;
        }

        _cards.Remove(card);

        RemoveContainer(card);
        return true;
    }

    private void RemoveContainer(GameObject card)
    {
        Card cardScript = card.GetComponent<Card>();

        _containers.Remove(card.GetComponent<Card>().AnchorContainer);
        Destroy(cardScript.AnchorContainer);
    }

    public int GetCardCount()
    {
        return _cards.Count;
    }
}
