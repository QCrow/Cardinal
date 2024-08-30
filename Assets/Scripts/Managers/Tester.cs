using UnityEngine;

public class Tester : MonoBehaviour
{
    public GameObject BuildingCardPrefab;
    public Canvas UICanvas;
    public GameObject Hand;

    public GameObject Target;

    private GameObject _card;

    void Start()
    {
        _card = Instantiate(BuildingCardPrefab, Hand.transform);
        BuildingCard buildingCard = _card.GetComponent<BuildingCard>();
        buildingCard.Initialize(1, "Test Card", "Nothing fancy, seems just to be a test card.", ColorType.YELLOW);

        _card = Instantiate(BuildingCardPrefab, Hand.transform);
        buildingCard = _card.GetComponent<BuildingCard>();
        buildingCard.Initialize(1, "Test Card", "Nothing fancy, seems just to be a test card.", ColorType.YELLOW);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TransformUtil.MoveTo(_card, Target);
            _card.transform.SetParent(Target.transform);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ResourceManager.Instance.ModifyResource(ResourceType.Food, 10);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            ResourceManager.Instance.ModifyResource(ResourceType.Energy, 10);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ResourceManager.Instance.ModifyResource(ResourceType.Morale, 10);
        }

        // Decrease resources
        if (Input.GetKeyDown(KeyCode.S))
        {
            ResourceManager.Instance.ModifyResource(ResourceType.Food, -10);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ResourceManager.Instance.ModifyResource(ResourceType.Energy, -10);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ResourceManager.Instance.ModifyResource(ResourceType.Morale, -10);
        }
    }
}