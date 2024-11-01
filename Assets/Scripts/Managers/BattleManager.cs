using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Text;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    static public BattleManager Instance { get; private set; }

    [Header("Enemy Health")]
    public int EnemyMaxHealth;
    public int EnemyCurrentHealth;
    [SerializeField] private HealthBar _healthBar;
    public int LastDealtDamage = 0;

    [SerializeField] private TMP_Text _bossDescriptionText;

    [Header("Attack")]
    [SerializeField] private int _totalAttackCount = 3;
    [SerializeField] private int _remainingAttackCount;
    [SerializeField] private TMP_Text _totalAttackText;
    public TMP_Text AttackCounterTextField;

    [Header("Redeploy")]
    [SerializeField] private int _redeployCountPerTurn = 2;
    [SerializeField] private int _remainingRedeployCount;
    public TMP_Text RedeployCounterTextField;

    [Header("Move")]
    [SerializeField] private int _moveCountPerTurn = 3;
    [SerializeField] private int _remainingMoveCount;
    public int RemainingMoveCount => _remainingMoveCount;
    public TMP_Text MoveCounterTextField;

    [Header("Buttons")]
    public Button AttackButton;
    public Button RedeployButton;
    public Button ResetButton;
    public TMP_Text ResetButtonTextField;

    public IBattlePhase CurrentBattlePhase { get; private set; }

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
        RedeployButton.onClick.AddListener(OnDeployButtonPressed);
        AttackButton.onClick.AddListener(OnAttackButtonPressed);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangePhase(new RewardPhase());
        }
    }

    public void StartBattleAgainstEnemy(EnemyScriptable enemyScriptable)
    {
        EnemyMaxHealth = enemyScriptable.MaxHealth;
        EnemyCurrentHealth = EnemyMaxHealth;
        UpdateEnemyHealth();

        ResetAttackCounter();
        ResetRedeployCounter();

        if (CurrentBattlePhase != null)
        {
            Board.Instance.ClearBoard();
        }


        StartCoroutine(WaitForBoard());

        IEnumerator WaitForBoard()
        {
            yield return new WaitUntil(() => Board.Instance.GetAllSlots().Count == 9);
        }

        enemyScriptable.Initialize();
        enemyScriptable.ApplyEffect(); // TODO: Change this to only apply effects on battle start
        CardManager.Instance.ResetDecks();

        _bossDescriptionText.text = enemyScriptable.Description;
        ChangePhase(new WaitPhase());
    }

    public void StartBattleAgainstEnemy(string EnemyName)
    {
        EnemyScriptable enemyScriptable = Resources.Load<EnemyScriptable>($"Enemies/{EnemyName}");
        StartBattleAgainstEnemy(enemyScriptable);
    }

    public void SetTotalAttack()
    {
        int totalAttack = Board.Instance.DeployedCards.Where(card => card != null).Sum(card => card.TotalAttack);
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
        SetTotalAttack();
    }

    public void OnDeployButtonPressed()
    {
        if (CurrentBattlePhase is ControlPhase)
        {
            Deploy();
            DecrementRedeployCounter();
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
        if (CurrentBattlePhase is ControlPhase)
        {
            Board.Instance.RestoreFromSnapshot();
            ResetMoveCounter();

            ApplyWhileInPlayEffects();
            Board.Instance.DeployedCards.ForEach(card => card.UpdateAttackValue());
            SetTotalAttack();
        }
        else
        {
            Deploy();
            ChangePhase(new ControlPhase());
        }
    }

    private void Deploy()
    {
        Board.Instance.ClearBoard();
        CardManager.Instance.Reshuffle();
        ResetMoveCounter();

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

        ApplyWhileInPlayEffects();
        Board.Instance.DeployedCards.ForEach(card => card.UpdateAttackValue());
        SetTotalAttack();
    }

    private void UpdateRedeployCounterDisplay()
    {
        RedeployCounterTextField.text = _remainingRedeployCount.ToString();
        if (_remainingRedeployCount == 0)
        {
            ToggleRedeployButton(false);
        }
    }

    private void UpdateAttackCounterDisplay()
    {
        AttackCounterTextField.text = _remainingAttackCount.ToString();
        if (_remainingAttackCount == 0)
        {
            ToggleAttackButton(false);
        }
    }

    private void UpdateMoveCounterDisplay()
    {
        MoveCounterTextField.text = _remainingMoveCount.ToString();
    }

    private void Attack()
    {
        LastDealtDamage = 0;
        Board.Instance.DeployedCards.ForEach(card =>
        {
            card.ApplyEffect(TriggerType.BeforeAttack);
        });

        Board.Instance.DeployedCards.ForEach(card =>
        {
            card.ApplyEffect(TriggerType.OnAttack);

            // Strike count is the number of times the card will attack, determined by the MultiStrike modifier
            int strikeCount = card.GetModifierByType(CardModifierType.MultiStrike) > 0 ? card.GetModifierByType(CardModifierType.MultiStrike) : 1;
            for (int i = 0; i < strikeCount; i++) InflictDamage(card.TotalAttack);
        });

        Board.Instance.DeployedCards.ForEach(card =>
        {
            card.ApplyEffect(TriggerType.AfterAttack);
        });

        Board.Instance.DeployedCards.ForEach(card => card.UpdateAttackValue());
    }

    private void InflictDamage(int damage)
    {
        LastDealtDamage += damage;
        EnemyCurrentHealth -= damage;

        UpdateEnemyHealth();
    }

    private void UpdateEnemyHealth()
    {
        _healthBar.SetHealth(EnemyCurrentHealth, EnemyMaxHealth);
    }

    public void ApplyWhileInPlayEffects()
    {
        foreach (Card card in Board.Instance.DeployedCards)
        {
            card.ApplyEffect(TriggerType.PrioWhileInPlay);
        }

        foreach (Card card in Board.Instance.DeployedCards)
        {
            card.ApplyEffect(TriggerType.WhileInPlay);
        }

        Board.Instance.DeployedCards.ForEach(card => card.UpdateAttackValue());
    }

    public void RevertWhileInPlayEffects()
    {
        foreach (Card card in Board.Instance.DeployedCards)
        {
            card.RevertEffect(TriggerType.WhileInPlay);
        }

        foreach (Card card in Board.Instance.DeployedCards)
        {
            card.RevertEffect(TriggerType.PrioWhileInPlay);
        }
    }

    public void ToggleRedeployButton(bool value)
    {
        if (value && _remainingRedeployCount <= 0)
        {
            RedeployButton.interactable = false;
        }
        else
        {
            RedeployButton.interactable = value;
        }

        TMP_Text text = RedeployButton.transform.Find("Value").GetComponent<TMP_Text>();
        Color textColor = text.color;
        textColor.a = value ? 1 : 0.5f;
        text.color = textColor;
    }

    public void ToggleAttackButton(bool value)
    {
        if (value && _remainingAttackCount <= 0)
        {
            AttackButton.interactable = false;
            return;
        }
        else
        {
            AttackButton.interactable = value;
        }

        TMP_Text text = AttackButton.transform.Find("Value").GetComponent<TMP_Text>();
        Color textColor = text.color;
        textColor.a = value ? 1 : 0.5f;
        text.color = textColor;
    }
}