using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject _battleUI;
    [SerializeField] private GameObject _navigationUI;

    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private Button _deployButton;
    public Button DeployButton => _deployButton;
    [SerializeField] private Button _attackButton;
    public Button AttackButton => _attackButton;
    [SerializeField] private Button _resetButton;
    public Button ResetButton => _resetButton;
    [SerializeField] private Button _checkDeckButton;
    public Button CheckDeckButton => _checkDeckButton;

    [SerializeField] private TMP_Text _attackCounter;
    [SerializeField] private TMP_Text _deployCounter;
    [SerializeField] private TMP_Text _dischargeCounter;
    [SerializeField] private TMP_Text _remainingMoveCounter;
    [SerializeField] private TMP_Text _remainingAttackCounter;

    private List<Button> _arrowButtons = new();

    [SerializeField] private TMPro.TMP_Text _totalAttackText;

    [SerializeField] private GameObject _rewardsPanel;
    private List<RewardSlot> _rewardSlots;
    [SerializeField] private GameObject _deckVisualizerViewport;
    [SerializeField] private GameObject _deckVisualizer;
    private bool _isDeckVisualizerActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged.AddListener(HandleGameStateChanged);
        GameManager.Instance.OnHealthChanged.AddListener(HandleHealthChanged);

        _healthBar.SetHealth(GameManager.Instance.MaxHealth, GameManager.Instance.MaxHealth);
        _rewardSlots = new List<RewardSlot>(_rewardsPanel.GetComponentsInChildren<RewardSlot>());
        UpdateAttackCounter(GameManager.Instance.MaxAttacks);
        UpdateDeployCounter(GameManager.Instance.MaxDeployCount);
        _checkDeckButton.onClick.AddListener(ToggleDeckVisualizer);
    }

    private void HandleGameStateChanged(IGameState previousState, IGameState currentState)
    {
        Debug.Log("Game State Changed: " + currentState.GetType().Name);
        switch (currentState)
        {
            case WaitState _:
                _deployButton.interactable = true;
                _attackButton.interactable = false;
                SetArrowButtonsInteractable(false);
                SetRewardsPanelActive(false);
                break;
            case DeployState _:
                _deployButton.interactable = false;
                _attackButton.interactable = false;
                SetArrowButtonsInteractable(false);
                break;
            case ControlState _:
                _deployButton.interactable = true;
                _attackButton.interactable = true;
                _resetButton.interactable = true;
                SetArrowButtonsInteractable(true);
                break;
            case AttackState _:
                _deployButton.interactable = false;
                _attackButton.interactable = false;
                _resetButton.interactable = false;
                SetArrowButtonsInteractable(false);
                break;
            case RewardState _:
                SetRewardsPanelActive(true);
                break;
            default:
                break;
        }
    }

    private void HandleHealthChanged(int currHealth)
    {
        _healthBar.SetHealth(currHealth, GameManager.Instance.MaxHealth);
    }

    private void SetArrowButtonsInteractable(bool interactable)
    {
        foreach (var arrowButton in _arrowButtons)
        {
            arrowButton.interactable = interactable;
        }
    }

    public void AddArrowButton(Button arrowButton)
    {
        _arrowButtons.Add(arrowButton);
    }

    public void SetTotalAttack(int totalAttack)
    {
        _totalAttackText.text = $"{totalAttack}";
    }

    private void SetRewardsPanelActive(bool active)
    {
        _rewardsPanel.SetActive(active);
    }

    public void SetRewards(List<Reward> rewards)
    {
        for (int i = 0; i < _rewardSlots.Count; i++)
        {
            if (i < rewards.Count)
            {
                _rewardSlots[i].SetReward(rewards[i]);
            }
            else
            {
                _rewardSlots[i].SetReward(null);
            }
        }
    }

    public void RefreshAttackValue()
    {
        Board.Instance.DeployedCards.ForEach(card => card.UpdateAttackValue());
    }

    public void UpdateMoveCounter(int remainingMoveCount)
    {
        if (remainingMoveCount <= 0) SetArrowButtonsInteractable(false);
        else SetArrowButtonsInteractable(true);
        _remainingMoveCounter.text = $"{remainingMoveCount}";
    }

    public void UpdateAttackCounter(int remainingAttackCount)
    {
        _remainingAttackCounter.text = $"{remainingAttackCount}";
    }

    public void UpdateDeployCounter(int remainingDeployCount)
    {
        _deployCounter.text = $"{remainingDeployCount}";
    }
    private void ToggleDeckVisualizer()
    {
        // Toggle the visualizer's active state
        _isDeckVisualizerActive = !_isDeckVisualizerActive;
        _deckVisualizerViewport.SetActive(_isDeckVisualizerActive);

        if (_isDeckVisualizerActive)
        {
            // Populate the deck visualizer with cards when it becomes active
            PopulateDeckVisualizer();
            ScrollToTop();  // Ensure the ScrollRect starts at the top
        }
        else
        {
            // Clear the visualizer when it is hidden
            ClearDeckVisualizer();
        }
    }

    private void PopulateDeckVisualizer()
    {
        Transform gridLayout = _deckVisualizer.GetComponentInChildren<GridLayoutGroup>().transform;

        // Get the number of cards in the deck
        int cardAmt = CardManager.Instance.InstantiateDeckToParent(gridLayout);

        // Get necessary components
        GridLayoutGroup layoutGroup = gridLayout.GetComponent<GridLayoutGroup>();
        RectTransform viewportRect = _deckVisualizerViewport.GetComponent<RectTransform>();  // Use viewport's width
        RectTransform contentRect = _deckVisualizer.GetComponent<RectTransform>();

        // Extract layout parameters
        float leftPadding = layoutGroup.padding.left;
        float rightPadding = layoutGroup.padding.right;
        float cardWidth = layoutGroup.cellSize.x;
        float horizontalSpacing = layoutGroup.spacing.x;

        // Calculate available width within the viewport
        float availableWidth = viewportRect.rect.width - leftPadding - rightPadding;

        // Calculate how many cards can fit per row
        int cardsPerRow = Mathf.Max(1, Mathf.FloorToInt((availableWidth + horizontalSpacing) / (cardWidth + horizontalSpacing)));

        // Calculate the number of rows needed
        int numRows = Mathf.CeilToInt((float)cardAmt / cardsPerRow);

        // Calculate the total height of the content
        float upperPadding = layoutGroup.padding.top;
        float lowerPadding = layoutGroup.padding.bottom;
        float cardHeight = layoutGroup.cellSize.y;
        float verticalSpacing = layoutGroup.spacing.y;

        float newHeight = upperPadding + lowerPadding +
                          (numRows * cardHeight) +
                          ((numRows - 1) * verticalSpacing);

        // Apply the new height to the RectTransform
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, newHeight);

        // Force the layout to rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }

    private void ClearDeckVisualizer()
    {
        Transform gridLayout = _deckVisualizer.GetComponentInChildren<GridLayoutGroup>().transform;

        foreach (Transform child in gridLayout)
        {
            Destroy(child.gameObject);
        }
    }

    private void ScrollToTop()
    {
        // Set the normalized position to (0, 1) to scroll to the top
        _deckVisualizerViewport.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
    }

    public void ShowNavigationUI()
    {
        _battleUI.SetActive(false);
        _navigationUI.SetActive(true);
    }

    public void ShowBattleUI()
    {
        _battleUI.SetActive(true);
        _navigationUI.SetActive(false);
    }
}