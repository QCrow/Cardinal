using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public GameObject BuildingCardPrefab;
    public Canvas UICanvas;
    public GameObject Hand;

    private GameObject _card;

    [SerializeField] List<int> _initialCards = new();

    void Start()
    {
        CardManager.Instance.LoadCards();
        foreach (int cardID in _initialCards)
        {
            _card = Instantiate(BuildingCardPrefab, Hand.transform);
            CardData cardData = CardManager.Instance.GetCardDataByID(cardID);
            CardFactory.CreateCard(_card.GetComponent<BuildingCard>(), cardData);
            _card.transform.SetParent(Hand.transform); // TODO: Have a Hand data structure which holds the cards in hand
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.EndTurn();
        }
    }
}