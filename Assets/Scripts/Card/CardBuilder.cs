using UnityEngine;

public class CardBuilder : MonoBehaviour
{
    public static CardBuilder Instance { get; private set; }

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

    public CardView BuildCardView(AbstractCard cardData, Transform parent)
    {
        var cardInstance = new CardInstance(cardData);
        var cardView = Instantiate(_cardPrefab, parent).GetComponent<CardView>();
        cardView.Initialize(cardInstance);
        return cardView;
    }

    public CardView BuildCardView(int cardID, Transform parent)
    {
        var cardData = CardLibrary.GetCard(cardID);
        return BuildCardView(cardData, parent);
    }
}