using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent<IGameState, IGameState> OnStateChanged = new();
    public UnityEvent<int> OnHealthChanged = new();

    #region Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Object References
    public Transform FrontDisplay;
    #endregion

    #region Game State
    public IGameState CurrentState { get; private set; }

    public void ChangeState(IGameState newState)
    {
        OnStateChanged.Invoke(CurrentState, newState);
        CurrentState?.OnExit(this);

        CurrentState = newState;

        CurrentState?.OnEnter(this);
    }
    #endregion

    #region Game Logic
    [SerializeField] private int _maxHealth = 100;
    public int MaxHealth => _maxHealth;
    [SerializeField] private int _currHealth = 100;

    [SerializeField] private int _remainingMoveCount = 0;
    public int RemainingMoveCount
    {
        get => _remainingMoveCount;
        set
        {
            _remainingMoveCount = value;
            UIManager.Instance.UpdateMoveCounter(_remainingMoveCount);
        }
    }
    public int MovePerTurn = 3;

    public void InflictDamage(int damage)
    {
        _currHealth -= damage;
        OnHealthChanged.Invoke(_currHealth);
    }
    #endregion

    #region Game Loop
    private void Start()
    {
        // Initialize the board and start the game
        Board.Instance.Initialize();
        CardManager.Instance.InitializeDeck();

        // Initialize the boss HP
        _currHealth = _maxHealth;

        // After the board is initialized, start the game
        ChangeState(new WaitState());
    }
    #endregion
}
