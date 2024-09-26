using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private Button _deployButton;
    public Button DeployButton => _deployButton;
    [SerializeField] private Button _attackButton;
    public Button AttackButton => _attackButton;

    private List<Button> _arrowButtons = new();

    [SerializeField] private TMPro.TMP_Text _totalAttackText;

    [SerializeField] private GameObject _rewardsPanel;
    private List<RewardSlot> _rewardSlots;

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
    }

    private void HandleGameStateChanged(IGameState previousState, IGameState currentState)
    {
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
                SetArrowButtonsInteractable(true);
                break;
            case AttackState _:
                _deployButton.interactable = false;
                _attackButton.interactable = false;
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
}