using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardSystem : SerializedMonoBehaviour
{
    #region Singleton
    private static CardSystem _instance;
    public static CardSystem Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public CardRepository CardRepository { get; private set; }
    public DeckManager DeckManager { get; private set; }
    public CardRewardGenerator CardRewardGenerator { get; private set; }

    [SerializeField] private CardRandomGenerationConfig _randomGenerationConfig;
    [SerializeField] private StartingDeckScriptable _startingDeck;

    [SerializeField] private GameObject _cardPrefab;

    [SerializeField] private Transform _graveyard;
    public Transform Graveyard => _graveyard;

    #region Initialization
    private void InitializeManagers()
    {
        CardRepository = new CardRepository(_cardPrefab); // CardLoader automatically loads all cards from the "Cards" resource folder on construction
        DeckManager = new DeckManager(_startingDeck.Deck); // DeckManager automatically initializes the player's deck with the starting deck on construction
        CardRewardGenerator = new CardRewardGenerator(_randomGenerationConfig);
    }
    #endregion
}
