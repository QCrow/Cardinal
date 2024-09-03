using UnityEngine;

// TODO: Refector this
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameManager).Name);
                    _instance = singletonObject.AddComponent<GameManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private Card _selectedCard;
    public Card SelectedCard { get => _selectedCard; set => _selectedCard = value; }

    public void EndTurn()
    {
        Debug.Log("END TURN");
        EffectResolveManager.Instance.ResolveEndTurnEffects();
    }
}
