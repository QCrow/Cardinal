using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private Card _selectedCard;
    public Card SelectedCard { get => _selectedCard; set => _selectedCard = value; }

    public Camera UICamera; //TODO: To be removed and placed elsewhere
}
