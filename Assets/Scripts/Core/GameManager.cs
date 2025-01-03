using UnityEngine;

public enum GameState
{
    Map,
    Battle,
    Shop,
    MysteryEvent
}

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

    #region Game State
    public int CurrentLevel = 1;
    private GameState _currentGameState;
    public GameState CurrentGameState => _currentGameState;

    public void ChangeGameState(GameState gameState)
    {
        _currentGameState = gameState;
        switch (_currentGameState)
        {
            case GameState.Map:
                CanMove = true;
                UIManager.Instance.MapPanel.SetActive(true);
                UIManager.Instance.BattlePanel.SetActive(false);
                UIManager.Instance.ShopPanel.SetActive(false);
                UIManager.Instance.MysteryEventPanel.SetActive(false);
                break;
            case GameState.Battle:
                CanMove = false;
                UIManager.Instance.MapPanel.SetActive(false);
                UIManager.Instance.BattlePanel.SetActive(true);
                UIManager.Instance.ShopPanel.SetActive(false);
                UIManager.Instance.MysteryEventPanel.SetActive(false);
                break;
            case GameState.Shop:
                CanMove = false;
                UIManager.Instance.MapPanel.SetActive(false);
                UIManager.Instance.BattlePanel.SetActive(false);
                UIManager.Instance.ShopPanel.SetActive(true);
                UIManager.Instance.MysteryEventPanel.SetActive(false);
                break;
            case GameState.MysteryEvent:
                UIManager.Instance.MapPanel.SetActive(false);
                UIManager.Instance.BattlePanel.SetActive(false);
                UIManager.Instance.ShopPanel.SetActive(false);
                UIManager.Instance.MysteryEventPanel.SetActive(true);
                break;
        }
    }
    #endregion
}
