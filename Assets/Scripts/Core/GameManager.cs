using Map;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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

    public int Seed;

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

    private void Start()
    {

    }

    public int GetDerivedSeedWithPosition(int baseSeed, int multiplier, int offset)
    {
        var position = MapManager.Instance.CurrentNode.position;
        // Use the node's position (x, y) as part of the seed derivation
        int positionInfluence = Mathf.FloorToInt(position.x * 1000 + position.y * 1000); // Scale position values
        return baseSeed * multiplier + offset + positionInfluence;
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
                NavBarManager.Instance.MoveMapOnScreen();
                //UIManager.Instance.MapPanel.SetActive(true);
                if (MapManager.Instance.IsCurrentNodeLast())
                {
                    CurrentLevel += 1;
                    //TODO: if certain event can only appear in level 2, add them when update level
                    MapManager.Instance.LoadMap();
                }
                //check if its last node, if so load next level with seed
                UIManager.Instance.BattlePanel.SetActive(false);
                UIManager.Instance.ShopPanel.SetActive(false);
                UIManager.Instance.MysteryEventPanel.SetActive(false);
                MapPlayerTracker.Instance.UnlockNodes();
                break;
            case GameState.Battle:
                CanMove = false;
                //UIManager.Instance.MapPanel.SetActive(false);
                NavBarManager.Instance.MoveMapOffScreen();
                UIManager.Instance.BattlePanel.SetActive(true);
                UIManager.Instance.ShopPanel.SetActive(false);
                UIManager.Instance.MysteryEventPanel.SetActive(false);
                break;
            case GameState.Shop:
                CanMove = false;
                //UIManager.Instance.MapPanel.SetActive(false);
                NavBarManager.Instance.MoveMapOffScreen();
                UIManager.Instance.BattlePanel.SetActive(false);
                UIManager.Instance.ShopPanel.SetActive(true);
                UIManager.Instance.MysteryEventPanel.SetActive(false);
                break;
            case GameState.MysteryEvent:
                //UIManager.Instance.MapPanel.SetActive(false);
                NavBarManager.Instance.MoveMapOffScreen();
                UIManager.Instance.BattlePanel.SetActive(false);
                UIManager.Instance.ShopPanel.SetActive(false);
                UIManager.Instance.MysteryEventPanel.SetActive(true);
                break;
        }
    }
    #endregion
}
