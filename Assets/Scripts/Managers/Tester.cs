using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public GameObject BuildingCardPrefab;
    public Canvas UICanvas;
    public GameObject HandCanvas;

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
            if (!Hand.Instance.AddCardByID(cardID))
            {
                Debug.Log("Cannot add card to hand");
            }
        }
    }
#nullable disable

    private void Update()
    {
        if (!InputManager.Instance.CanProcessGameInput())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.EndTurn();
        }
    }
}
