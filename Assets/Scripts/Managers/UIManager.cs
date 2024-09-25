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

    [SerializeField] private List<Button> _arrowButtons;

    [SerializeField] private TMPro.TMP_Text _totalAttackText;

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
    }

    private void HandleGameStateChanged(IGameState previousState, IGameState currentState)
    {
        switch (currentState)
        {
            case WaitState _:
                _deployButton.interactable = true;
                _attackButton.interactable = false;
                SetArrowButtonsInteractable(false);
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
}