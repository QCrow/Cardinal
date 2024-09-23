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

    #region Game State
    public IGameState CurrentState { get; private set; }
    public UnityEvent<IGameState, IGameState> OnStateChange;

    private void OnEnable()
    {
        if (OnStateChange == null)
        {
            OnStateChange = new UnityEvent<IGameState, IGameState>();
        }
    }

    public void ChangeState(IGameState newState)
    {
        IGameState previousState = CurrentState;
        CurrentState?.OnExit(this);

        CurrentState = newState;

        CurrentState?.OnEnter(this);

        OnStateChange?.Invoke(previousState, CurrentState);
    }
    #endregion

    #region Game Loop
    private void Start()
    {
        ChangeState(new WaitState());
    }
    #endregion

    #region Game Logic
    private int _monsterHP = 100;

    public void AttackMonster(int damage)
    {
        _monsterHP -= damage;
    }
    #endregion
}
