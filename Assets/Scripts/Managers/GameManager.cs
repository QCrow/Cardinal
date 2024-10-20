using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool CanMove
    {
        get => _canMove;
        set
        {
            _canMove = value;
            UIManager.Instance.SetArrowButtonsInteractable(_canMove);
        }
    }

    private bool _canMove = false;

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

    #region Object References
    public Transform FrontDisplay;
    #endregion

    #region Game State
    [SerializeField] private bool _isNavigating = false;
    public bool IsNavigating => _isNavigating;

    public IBattlePhase CurrentState { get; private set; }

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
        }
    }
    #endregion

    #region Game Loop
    private void Start()
    {
        CardManager.Instance.InitializeDeck();

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
