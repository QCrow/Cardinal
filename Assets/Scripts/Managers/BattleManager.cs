using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    static public BattleManager Instance { get; private set; }

    public int EnemyMaxHealth;
    public int EnemyCurrentHealth;

    public IBattlePhase CurrentBattlePhase { get; private set; }

    [SerializeField] private int _redeployCountPerTurn = 2;
    private int _remainingRedeployCount;

    [SerializeField] private int _totalAttackCount = 3;
    private int _remainingAttackCount;

    [SerializeField] private int _moveCountPerTurn = 3;
    private int _remainingMoveCount;
    public int RemainingMoveCount => _remainingMoveCount;

    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private TMP_Text _totalAttackText;

    public Button DeployButton;
    public TMP_Text DeployButtonTextField;
    public GameObject RedeployCounter;
    public TMP_Text RedeployCounterTextField;
    //TODO: Put move counter reference here
    //TODO: Check with designer if we need a discharge counter

    public Button AttackButton;
    public TMP_Text AttackCounterTextField;

    public TMP_Text MoveCounterTextField;

    public Button ResetButton;

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
        DeployButton.onClick.AddListener(OnDeployButtonPressed);
        AttackButton.onClick.AddListener(OnAttackButtonPressed);
    }

    public void StartBattleAgainstEnemy(EnemyScriptable enemyScriptable)
    {
        EnemyMaxHealth = enemyScriptable.MaxHealth;
        EnemyCurrentHealth = EnemyMaxHealth;
        UpdateEnemyHealth();

        ResetAttackCounter();
        //TODO: Trigger all battle start events such as artifact resolution
        ChangePhase(new WaitPhase());
    }

    public void SetTotalAttack()
    {
        int totalAttack = Board.Instance.DeployedCards.Sum(card => card.TotalAttack);
        _totalAttackText.text = $"{totalAttack}";
    }

    public void ChangePhase(IBattlePhase newBattlePhase)
    {
        CurrentBattlePhase?.OnExit();

        CurrentBattlePhase = newBattlePhase;

        CurrentBattlePhase?.OnEnter();
    }

    public void ResetRedeployCounter()
    {
        _remainingRedeployCount = _redeployCountPerTurn;
        UpdateRedeployCounterDisplay();
    }

    public void ResetAttackCounter()
    {
        _remainingAttackCount = _totalAttackCount;
        UpdateAttackCounterDisplay();
    }

    public void ResetMoveCounter()
    {
        _remainingMoveCount = _moveCountPerTurn;
        if (_remainingMoveCount > 0)
        {
            GameManager.Instance.CanMove = true;
        }
        UpdateMoveCounterDisplay();
    }

    public void DecrementAttackCounter()
    {
        _remainingAttackCount--;
        UpdateAttackCounterDisplay();
    }

    public void DecrementRedeployCounter()
    {
        _remainingRedeployCount--;
        UpdateRedeployCounterDisplay();
    }

    public void DecrementMoveCounter()
    {
        _remainingMoveCount--;
        if (_remainingMoveCount == 0)
        {
            GameManager.Instance.CanMove = false;
        }
        UpdateMoveCounterDisplay();
    }

    public void OnDeployButtonPressed()
    {
        Deploy();
        if (CurrentBattlePhase is ControlPhase)
        {
            DecrementRedeployCounter();
        }
        else
        {
            ChangePhase(new ControlPhase());
        }
    }

    public void OnAttackButtonPressed()
    {
        AttackButton.interactable = false;
        DecrementAttackCounter();
        Attack();
        if (EnemyCurrentHealth <= 0)
        {
            ChangePhase(new RewardPhase());
        }
        else if (_remainingAttackCount == 0)
        {
            ChangePhase(new GameOverPhase());
        }
        else
        {
            ChangePhase(new WaitPhase());
        }
    }

    public void OnResetButtonPressed()
    {
        Board.Instance.RestoreFromSnapshot();
        ResetMoveCounter();
    }

    private void Deploy()
    {
        Board.Instance.ClearBoard();
        CardManager.Instance.Reshuffle();

        List<Card> _cards = new();

        // Place cards randomly on the board
        while (true)
        {
            Slot slot = Board.Instance.GetRandomEmptySlot();
            if (slot == null) break;

            // Instantiate the card and bind it to the slot
            Card card = CardManager.Instance.DrawCard();
            if (card == null) break;

            card.ResetTemporaryState();
            card.BindToSlot(slot);

            // Apply the deploy trigger effects
            card.ApplyEffect(TriggerType.OnDeploy);
            _cards.Add(card);
        }
        Board.Instance.DeployedCards = _cards;
        Board.Instance.SaveSnapshot();
    }

    private void UpdateRedeployCounterDisplay()
    {
        RedeployCounterTextField.text = _remainingRedeployCount.ToString();
        if (_remainingRedeployCount == 0)
        {
            DeployButton.interactable = false;
        }
    }

    private void UpdateAttackCounterDisplay()
    {
        AttackCounterTextField.text = _remainingAttackCount.ToString();
    }

    private void UpdateMoveCounterDisplay()
    {
        MoveCounterTextField.text = _remainingMoveCount.ToString();
    }

    private void Attack()
    {
        Board.Instance.DeployedCards.ForEach(card =>
        {
            card.ApplyEffect(TriggerType.OnAttack);

            // Strike count is the number of times the card will attack, determined by the MultiStrike modifier
            int strikeCount = card.GetModifierByType(ModifierType.MultiStrike) > 0 ? card.GetModifierByType(ModifierType.MultiStrike) : 1;
            for (int i = 0; i < strikeCount; i++) InflictDamage(card.TotalAttack);
        });

    }

    private void InflictDamage(int damage)
    {
        EnemyCurrentHealth -= damage;
        UpdateEnemyHealth();
    }

    private void UpdateEnemyHealth()
    {
        _healthBar.SetHealth(EnemyCurrentHealth, EnemyMaxHealth);
    }
}