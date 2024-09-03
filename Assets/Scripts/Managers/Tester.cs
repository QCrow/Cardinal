using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public GameObject BuildingCardPrefab;
    public Canvas UICanvas;
    public GameObject Hand;

    private GameObject _card;

    [SerializeField] List<int> _initialCards = new();

#nullable enable
    async void Start()
    {
        // Load cards asynchronously and wait for it to complete
        await CardManager.Instance.LoadCardsAsync();

        // Proceed with card generation after loading is complete
        foreach (int cardID in _initialCards)
        {
            _card = Instantiate(BuildingCardPrefab, Hand.transform);
            CardData? cardData = CardManager.Instance.GetCardDataByID(cardID);
            if (cardData != null)
            {
                CardFactory.CreateCard(_card.GetComponent<BuildingCard>(), cardData);
                _card.transform.SetParent(Hand.transform); // TODO: Have a Hand data structure which holds the cards in hand
            }
            else
            {
                Debug.LogWarning($"Card with ID {cardID} could not be found.");
            }
        }
    }
#nullable disable

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.EndTurn();
        }
    }
}
