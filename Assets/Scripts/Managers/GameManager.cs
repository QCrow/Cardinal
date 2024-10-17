using UnityEngine;
using UnityEngine.Events;
using static UnityEditorInternal.VersionControl.ListControl;

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
    [SerializeField] private bool _isNavigating = false;
    public bool IsNavigating => _isNavigating;

    public IGameState CurrentState { get; private set; }

    public void ChangeState(IGameState newState)
    {
        OnStateChanged.Invoke(CurrentState, newState);
        CurrentState?.OnExit(this);

        CurrentState = newState;

        CurrentState?.OnEnter(this);
    }

    public void ChangeNavigationState(bool isNavigating)
    {
        _isNavigating = isNavigating;
        if (_isNavigating)
        {
            UIManager.Instance.ShowNavigationUI();
        }
        else
        {
            UIManager.Instance.ShowBattleUI();
            ChangeState(new WaitState());
        }
    }
    #endregion

    #region Game Logic
    [SerializeField] private int _maxHealth = 100;
    public int MaxHealth => _maxHealth;
    [SerializeField] private int _currHealth = 100;
    public int CurrentHealth => _currHealth;
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

    [SerializeField] private int _maxDeployCount = 2;
    public int MaxDeployCount => _maxDeployCount;
    public int RemainingDeployCount;

    public void DecrementDeployCount()
    {
        if (RemainingDeployCount > 0)
        {
            RemainingDeployCount--;
        }
    }

    public void ResetDeployCount()
    {
        RemainingDeployCount = _maxDeployCount;
    }

    public bool CanDeploy() => RemainingDeployCount > 0;

    [SerializeField] private int _maxAttacks = 5;
    public int MaxAttacks => _maxAttacks;
    public int _remainingAttacks;
    public void ResetAttackCounter()
    {
        _remainingAttacks = _maxAttacks;
    }

    public void DecrementAttackCounter()
    {
        _remainingAttacks--;
        UIManager.Instance?.UpdateAttackCounter(_remainingAttacks);
    }

    public void InflictDamage(int damage)
    {
        _currHealth -= damage;
        OnHealthChanged.Invoke(_currHealth);
    }

    public int GetRemainingAttacks() => _remainingAttacks;
    #endregion

    #region Game Loop
    private void Start()
    {
        // Initialize the board and start the game
        CardManager.Instance.InitializeDeck();
        
        // Initialize the boss HP
        _currHealth = _maxHealth;
        ResetAttackCounter();  // Initialize the attack counter
        ResetDeployCount();  // Reset the deploy counter

        ChangeNavigationState(false);
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeNavigationState(!_isNavigating);
        }
    }
}
