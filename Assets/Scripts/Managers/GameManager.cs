using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Singleton
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
    #endregion

    #region Object References
    [SerializeField] private HealthBar _healthBar;
    public Transform FrontDisplay;
    #endregion

    #region Game State
    public IGameState CurrentState { get; private set; }

    public void ChangeState(IGameState newState)
    {
        IGameState previousState = CurrentState;
        CurrentState?.OnExit(this);

        CurrentState = newState;

        CurrentState?.OnEnter(this);
    }
    #endregion

    #region Game Logic
    [SerializeField] private int _monsterMaxHealth = 100;
    [SerializeField] private int _monsterCurrentHealth = 100;

    public void AttackMonster(int damage)
    {
        _monsterCurrentHealth -= damage;
        _healthBar.SetHealth(_monsterCurrentHealth, _monsterMaxHealth);

        // if (_monsterCurrentHealth <= 0)
        // {
        //     ChangeState(new GameOverState());
        // }
    }
    #endregion

    #region Game Loop
    private void Start()
    {
        // Initialize the board and start the game
        Board.Instance.Initialize();
        CardManager.Instance.InitializeDeck();
        _monsterCurrentHealth = _monsterMaxHealth;
        _healthBar.SetHealth(_monsterCurrentHealth, _monsterMaxHealth);

        // After the board is initialized, start the game
        ChangeState(new WaitState());
    }
    #endregion
}
