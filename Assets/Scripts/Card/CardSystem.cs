using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    public static CardSystem Instance { get; private set; }

    [SerializeField] private CardGenConfig _randomGenerationConfig;
    public CardRewardGenerator RewardGenerator { get; private set; }

    public Transform GraveyardTransform;

    [SerializeField] private GameObject _cardPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RewardGenerator = new CardRewardGenerator(_randomGenerationConfig);
    }

    public CardInstance BuildCardInstance(AbstractCard cardData, Transform parent = null)
    {
        var cardInstance = Instantiate(_cardPrefab, parent).GetComponent<CardInstance>();
        Debug.Log($"Building card instance for card ID {cardData.ID}");
        cardInstance.Initialize(cardData);
        return cardInstance;
    }

    public CardInstance BuildCardInstance(int cardID, Transform parent = null)
    {
        var cardData = CardLibrary.GetCard(cardID);
        if (cardData == null)
        {
            Debug.LogError($"Card with ID {cardID} not found in library.");
            return null;
        }
        return BuildCardInstance(cardData, parent);
    }

    public CardInstance BuildCardInstance(CardInstance cardInstance, Transform parent = null)
    {
        var newCardInstance = Instantiate(_cardPrefab, parent).GetComponent<CardInstance>();
        newCardInstance.Initialize(cardInstance);
        return newCardInstance;
    }
}