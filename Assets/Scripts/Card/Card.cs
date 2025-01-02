using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

/// <summary>
/// Represents a card in the game with various attributes and behaviors.
/// </summary>
public class Card : SlotContent, IPointerEnterHandler, IPointerExitHandler
{
    #region Card Base Data

    [BoxGroup("Card Base Data")]
    public int ID { get; private set; }

    [BoxGroup("Card Base Data")]
    public CardRarityType Rarity { get; private set; }

    [BoxGroup("Card Base Data")]
    public string CardName { get; private set; }

    [BoxGroup("Card Base Data")]
    public int BaseAttack { get; private set; }

    // Removed: Amount in deck, Price, isSold, isInShop
    // These are to be moved to a dedicated decorator class (IPurchasable)

    [BoxGroup("Card Base Data")]
    private Dictionary<TriggerType, List<ConditionalEffect>> _conditionalEffects = new();

    #endregion

    #region Card Runtime State

    // Modifier storage with persistence levels
    private Dictionary<ModifierPersistenceType, Dictionary<CardModifierType, int>> _modifiers = new();

    /// <summary>
    /// Gets the total attack value of the card, considering base attack and modifiers.
    /// </summary>
    public int TotalAttack => CalculateTotalAttack();

    private int CalculateTotalAttack()
    {
        if (CurrentSlot != null && CurrentSlot.GetModifierValue(SlotModifierType.NoDamage) > 0)
        {
            return 0;
        }

        int strength = GetModifierValue(CardModifierType.Strength);
        int weakness = GetModifierValue(CardModifierType.Weakness);
        return Math.Max(0, BaseAttack + strength - weakness);
    }

    /// <summary>
    /// Gets the total modifier value of a specific type, considering all persistence levels.
    /// </summary>
    /// <param name="type">The type of modifier to get the value of.</param>
    public int GetModifierValue(CardModifierType type)
    {
        int total = 0;
        foreach (var persistenceDict in _modifiers.Values)
        {
            if (persistenceDict.TryGetValue(type, out int value))
            {
                total += value;
            }
        }
        return total;
    }

    #endregion

    #region UI References

    [BoxGroup("UI References")]
    [SerializeField] private TMP_Text _cardNameText;

    [BoxGroup("UI References")]
    [SerializeField] private GameObject _descriptionContainer;

    [BoxGroup("UI References")]
    [SerializeField] private TMP_Text _descriptionText;

    [BoxGroup("UI References")]
    [SerializeField] private TMP_Text _attackValueText;

    [BoxGroup("UI References")]
    [SerializeField] private GameObject _cycleContainer;

    [BoxGroup("UI References")]
    [SerializeField] private TMP_Text _currentCycleValueText;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        HideDescription();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the card with data from a scriptable object and conditional effects.
    /// </summary>
    /// <param name="cardScriptable">The scriptable object containing card data.</param>
    /// <param name="conditionalEffects">The conditional effects associated with the card.</param>
    public void Initialize(CardScriptable cardScriptable, Dictionary<TriggerType, List<ConditionalEffect>> conditionalEffects)
    {
        ID = cardScriptable.ID;
        Rarity = cardScriptable.Rarity;
        CardName = cardScriptable.Name;
        BaseAttack = cardScriptable.BaseAttack;
        _conditionalEffects = conditionalEffects;

        SetupCycleContainer();
        SetupUI(cardScriptable);
    }

    private void SetupCycleContainer()
    {
        // Check if any of the conditional effects are cycle conditions under all triggers
        bool hasCycleCondition = _conditionalEffects
            .SelectMany(effect => effect.Value)
            .OfType<CycleCondition>()
            .Any();

        _cycleContainer.SetActive(hasCycleCondition);
    }

    private void SetupUI(CardScriptable cardScriptable)
    {
        _cardNameText.text = CardName;
        _descriptionText.text = cardScriptable.Description; // TODO: Make this into a description class with tooltips
        _attackValueText.text = TotalAttack.ToString();
    }

    #endregion

    #region UI Update Methods

    /// <summary>
    /// Updates the attack value displayed on the card.
    /// </summary>
    public void UpdateAttackValue()
    {
        _attackValueText.text = TotalAttack.ToString();
    }

    /// <summary>
    /// Updates the cycle value displayed on the card.
    /// </summary>
    /// <param name="currentCycle">The current cycle value.</param>
    public void UpdateCycleValue(int currentCycle)
    {
        _currentCycleValueText.text = currentCycle.ToString();
    }

    private void ShowDescription()
    {
        _descriptionContainer.SetActive(true);
        EnsureDescriptionCanvas();
    }

    private void HideDescription()
    {
        _descriptionContainer.SetActive(false);
    }

    private void EnsureDescriptionCanvas()
    {
        // Ensure the description container has a canvas component
        if (!_descriptionContainer.TryGetComponent<Canvas>(out var canvas))
        {
            canvas = _descriptionContainer.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "UI";
        }
        canvas.sortingOrder = 200; // Ensure on top
    }

    #endregion

    #region Event Handlers

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowDescription();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideDescription();
    }


    #endregion

    #region Effect Application

    /// <summary>
    /// Applies effects based on a trigger.
    /// </summary>
    /// <param name="trigger">The trigger type.</param>
    public void ApplyEffect(TriggerType trigger)
    {
        if (!_conditionalEffects.TryGetValue(trigger, out List<ConditionalEffect> effects))
            return;

        foreach (var effect in effects)
        {
            effect.ApplyEffect();
        }
    }

    /// <summary>
    /// Reverts effects based on a trigger.
    /// </summary>
    /// <param name="trigger">The trigger type.</param>
    public void RevertEffect(TriggerType trigger)
    {
        if (!_conditionalEffects.TryGetValue(trigger, out List<ConditionalEffect> effects))
            return;

        foreach (var effect in effects)
        {
            effect.RevertEffect();
        }
    }

    #endregion

    #region Slot Interaction

    public override void BindToSlot(Slot slot)
    {
        base.BindToSlot(slot);
        UpdateAttackValue();
    }

    #endregion

    #region State Management
    /// <summary>
    /// Resets all card's modifiers under a specified persistence level.
    /// </summary>
    /// <param name="persistence"> The persistence level to reset.</param>
    public void ResetCardModifierState(ModifierPersistenceType persistence)
    {
        // Determine which modifier persistence levels need to be cleared
        List<ModifierPersistenceType> persistencesToClear = persistence switch
        {
            ModifierPersistenceType.Turn => new List<ModifierPersistenceType>
        {
                ModifierPersistenceType.Turn
        },
            ModifierPersistenceType.Battle => new List<ModifierPersistenceType>
        {
            ModifierPersistenceType.Turn,
            ModifierPersistenceType.Battle
        },
            ModifierPersistenceType.Chapter => new List<ModifierPersistenceType>
        {
            ModifierPersistenceType.Turn,
            ModifierPersistenceType.Battle,
            ModifierPersistenceType.Chapter
        },
            ModifierPersistenceType.Permanent => new List<ModifierPersistenceType>
        {
            ModifierPersistenceType.Turn,
            ModifierPersistenceType.Battle,
            ModifierPersistenceType.Chapter,
            ModifierPersistenceType.Permanent
        },
            _ => new List<ModifierPersistenceType>() // Default case, if needed
        };

        // Clear the determined modifier persistence levels
        foreach (var p in persistencesToClear)
        {
            ClearModifiers(p);
        }

        // If the persistence type is Chapter or Permanent, reset cycle conditions if active
        if (persistence is ModifierPersistenceType.Chapter or ModifierPersistenceType.Permanent)
        {
            if (_cycleContainer.activeSelf)
            {
                foreach (var condition in _conditionalEffects.SelectMany(effect => effect.Value).OfType<CycleCondition>())
                {
                    condition.ResetCycle();
                }
            }
        }
    }

    /// <summary>
    /// Adds a modifier to the card with a specified persistence.
    /// </summary>
    /// <param name="type">The type of modifier.</param>
    /// <param name="amount">The amount of the modifier.</param>
    /// <param name="persistence">The persistence level of the modifier.</param>
    public void AddModifier(CardModifierType type, int amount, ModifierPersistenceType persistence)
    {
        if (!_modifiers.ContainsKey(persistence))
        {
            _modifiers[persistence] = new Dictionary<CardModifierType, int>();
        }

        if (_modifiers[persistence].ContainsKey(type))
        {
            _modifiers[persistence][type] += amount;
        }
        else
        {
            _modifiers[persistence][type] = amount;
        }

        UpdateAttackValue();
    }

    /// <summary>
    /// Removes a modifier from the card based on its type and persistence.
    /// </summary>
    /// <param name="type">The type of modifier.</param>
    /// <param name="amount">The amount to remove.</param>
    /// <param name="persistence">The persistence level of the modifier.</param>
    public void RemoveModifier(CardModifierType type, int amount, ModifierPersistenceType persistence)
    {
        if (_modifiers.ContainsKey(persistence) && _modifiers[persistence].ContainsKey(type))
        {
            _modifiers[persistence][type] -= amount;
            if (_modifiers[persistence][type] <= 0)
            {
                _modifiers[persistence].Remove(type);
                if (_modifiers[persistence].Count == 0)
                {
                    _modifiers.Remove(persistence);
                }
            }

            UpdateAttackValue();
        }
    }

    private void ClearModifiers(ModifierPersistenceType persistence)
    {
        if (_modifiers.ContainsKey(persistence))
        {
            _modifiers.Remove(persistence);
        }
    }
    #endregion

    #region Serialization
    public CardSaveData GetSaveData()
    {
        CardSaveData saveData = new CardSaveData
        (
            this.gameObject.GetInstanceID(),
            CurrentSlot != null ? CurrentSlot.Row : -1,
            CurrentSlot != null ? CurrentSlot.Col : -1,
            ID,
            _conditionalEffects.SelectMany(effect => effect.Value).OfType<CycleCondition>().FirstOrDefault()?.CycleValue ?? 0,
            _modifiers.ToDictionary(pair => pair.Key, pair => pair.Value.ToDictionary(pair => pair.Key, pair => pair.Value))
        );

        return saveData;
    }

    public void LoadFromSaveData(CardSaveData saveData)
    {
        ID = saveData.ID;
        _modifiers = saveData.Modifiers;
        if (_conditionalEffects.SelectMany(effect => effect.Value).OfType<CycleCondition>().FirstOrDefault() is CycleCondition cycleCondition)
        {
            cycleCondition.CycleValue = saveData.CycleValue;
        }
        UpdateAttackValue();
        UpdateCycleValue(saveData.CycleValue);
    }
    #endregion
}
