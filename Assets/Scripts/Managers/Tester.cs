using UnityEngine;

public class Tester : MonoBehaviour
{
    public GameObject BuildingCardPrefab;
    public Canvas UICanvas;
    public GameObject Hand;

    private GameObject _card;

    void Start()
    {
        // _card = Instantiate(BuildingCardPrefab, Hand.transform);
        // BuildingCard buildingCard = _card.GetComponent<BuildingCard>();
        // buildingCard.Initialize(1, "Test Card");

        // _card = Instantiate(BuildingCardPrefab, Hand.transform);
        // buildingCard = _card.GetComponent<BuildingCard>();
        // buildingCard.Initialize(1, "Test Card");

        // CardManager.Instance.LoadCards();
        // CardData data = CardManager.Instance.GetCardDataByID(1);
        // Debug.Log(data.CardName);
    }
}