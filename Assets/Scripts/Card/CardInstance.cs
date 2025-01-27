using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEditor.Localization.Editor;
using System.Collections;

public class CardInstance : SlotContent, IPointerEnterHandler, IPointerExitHandler
{
    public AbstractCard Template { get; private set; }

    public int TokenCount;
    public int ChargeCount;
    public int Cooldown;

    // Modifier storage with persistence levels
    protected Dictionary<ModifierPersistenceType, Dictionary<CardModifierType, int>> _modifiers = new();


    #region UI References
    [BoxGroup("UI References")]
    [SerializeField] private TMP_Text _cardNameText;

    [BoxGroup("UI References")]
    [SerializeField] private GameObject _descriptionContainer;

    [BoxGroup("UI References")]
    [SerializeField] private TMP_Text _descriptionText;
    #endregion

    public void Initialize(AbstractCard template)
    {
        Template = template;
        Debug.Log(Template.ID);
        string cardName = LocalizationHandler.Instance.GetCardName(Template.ID);
        _cardNameText.text = cardName;

        string description = LocalizationHandler.Instance.GetCardDescription(Template.ID);
        description = LocalizationHandler.Instance.ParseDynamicDescription(description, this);
        _descriptionText.text = description;
    }

    public void Initialize(CardInstance cardInstance)
    {
        _modifiers = new Dictionary<ModifierPersistenceType, Dictionary<CardModifierType, int>>(cardInstance._modifiers);
        Initialize(cardInstance.Template);
    }

    /// <summary>
    /// Gets the total modifier value of a specific type, considering all persistence levels.
    /// </summary>
    /// <param name="type">The type of modifier to get the value of.</param>
    public int GetModifierValue(CardModifierType type)
    {
        int total = 0;
        foreach (var dict in _modifiers.Values)
        {
            if (dict.TryGetValue(type, out int value))
            {
                total += value;
            }
        }
        return total;
    }

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

        UpdateDescription();
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
        }

        UpdateDescription();
    }

    public void ResetCardModifierState(ModifierPersistenceType persistence)
    {
        if (_modifiers.ContainsKey(persistence))
        {
            _modifiers.Remove(persistence);
        }

        UpdateDescription();
    }

    private void UpdateDescription()
    {
        string description = LocalizationHandler.Instance.GetCardDescription(Template.ID);
        description = LocalizationHandler.Instance.ParseDynamicDescription(description, this);
        _descriptionText.text = description;
    }

    public int GetEffectiveValue(string key)
    {
        switch (key)
        {
            case "Damage":
                return Template.BaseValues[key] + GetModifierValue(CardModifierType.Damage);
            case "Shield":
                return Template.BaseValues[key] + GetModifierValue(CardModifierType.Shield);
            case "Cooldown":
                return Template.BaseValues[key];
            case "Damage Scaling":
                return Template.BaseValues[key];
            default:
                Debug.LogError($"Key {key} from localization is not found.");
                return 0;
        }
    }

    public void ActivateCardEffect(TriggerType trigger)
    {
        Template.ActivateCardEffect(trigger, this);
    }

    public override void BindToSlot(Slot slot)
    {
        base.BindToSlot(slot);
    }

    public override void UnbindFromSlot()
    {
        base.UnbindFromSlot();
    }

    private void OnEnable()
    {
        HideDescription();
    }

    private void ShowDescription()
    {
        _descriptionContainer.SetActive(true);
        StartCoroutine(SetSortingOrder());
        IEnumerator SetSortingOrder()
        {
            yield return null;
            if (!_descriptionContainer.TryGetComponent<Canvas>(out var canvas))
            {
                canvas = _descriptionContainer.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingLayerName = "UI";
            }
            canvas.sortingOrder = 200; // Ensure on top
        }
    }

    private void HideDescription()
    {
        _descriptionContainer.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowDescription();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideDescription();
    }
}
