using UnityEngine;

public class Tester : MonoBehaviour
{
    public GameObject BuildingCardPrefab;
    public Canvas UICanvas;
    public GameObject Hand;

    private GameObject _card;

    void Start()
    {
        _card = Instantiate(BuildingCardPrefab, Hand.transform);
        CardManager.Instance.LoadCards();
        CardData cardData = CardManager.Instance.GetCardDataByID(101);
        CardFactory.CreateCard(_card.GetComponent<BuildingCard>(), cardData);
        _card.transform.SetParent(Hand.transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.EndTurn();
        }
    }
}