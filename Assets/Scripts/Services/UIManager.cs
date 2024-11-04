using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject MapPanel;
    public GameObject BattlePanel;
    public GameObject ShopPanel;
    public GameObject EventPanel;

    public Transform OverlayDisplay;

    [SerializeField] private List<Button> _arrowButtons = new();

    [SerializeField] private Button _checkDeckButton;

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
        _rewardSlots = new List<RewardSlot>(_rewardsPanel.GetComponentsInChildren<RewardSlot>());
        // _checkDeckButton.onClick.AddListener(ToggleDeckVisualizer);
    }

    public void SetArrowButtonsInteractable(bool interactable)
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

    public void SetRewardsPanelActive(bool active)
    {
        _rewardsPanel.SetActive(active);
    }

    public void SetRewards(List<CardReward> rewards)
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
            ClearDisplayOverlay();
        }
    }

    private void ClearDisplayOverlay()
    {
        Transform overlayTransform = UIManager.Instance.OverlayDisplay;

        // Loop through all children and destroy them
        for (int i = overlayTransform.childCount - 1; i >= 0; i--)
        {
            GameObject child = overlayTransform.GetChild(i).gameObject;
            Destroy(child);
        }
    }

    private void PopulateDeckVisualizer()
    {
        // Transform gridLayout = _deckVisualizer.GetComponentInChildren<GridLayoutGroup>().transform;

        // // Get the number of cards in the deck
        // int cardAmt = CardSystem.Instance.InstantiateDeckToParent(gridLayout);

        // // Get necessary components
        // GridLayoutGroup layoutGroup = gridLayout.GetComponent<GridLayoutGroup>();
        // RectTransform viewportRect = _deckVisualizerViewport.GetComponent<RectTransform>();  // Use viewport's width
        // RectTransform contentRect = _deckVisualizer.GetComponent<RectTransform>();

        // // Extract layout parameters
        // float leftPadding = layoutGroup.padding.left;
        // float rightPadding = layoutGroup.padding.right;
        // float cardWidth = layoutGroup.cellSize.x;
        // float horizontalSpacing = layoutGroup.spacing.x;

        // // Calculate available width within the viewport
        // float availableWidth = viewportRect.rect.width - leftPadding - rightPadding;

        // // Calculate how many cards can fit per row
        // int cardsPerRow = Mathf.Max(1, Mathf.FloorToInt((availableWidth + horizontalSpacing) / (cardWidth + horizontalSpacing)));

        // // Calculate the number of rows needed
        // int numRows = Mathf.CeilToInt((float)cardAmt / cardsPerRow);

        // // Calculate the total height of the content
        // float upperPadding = layoutGroup.padding.top;
        // float lowerPadding = layoutGroup.padding.bottom;
        // float cardHeight = layoutGroup.cellSize.y;
        // float verticalSpacing = layoutGroup.spacing.y;

        // float newHeight = upperPadding + lowerPadding +
        //                   (numRows * cardHeight) +
        //                   ((numRows - 1) * verticalSpacing);

        // // Apply the new height to the RectTransform
        // contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, newHeight);

        // // Force the layout to rebuild
        // LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
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
}