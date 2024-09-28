using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Card Base Data
    public int ID;
    public RarityType Rarity;
    public string Name;
    public HashSet<TraitType> Traits;

    public int BaseAttack;

    public Dictionary<TriggerType, List<ConditionalEffect>> ConditionalEffects = new();
    #endregion

    #region Card Runtime State
    public Slot Slot = null;
    private Dictionary<ModifierType, int> _modifiers = new();

    [SerializeField] private int _tempDamage = 0;
    public int TempDamage
    {
        get { return _tempDamage; }
        set
        {
            _tempDamage = value;
            UpdateAttackValue();
        }
    }
    // The total attack of the card, including base attack, temporary damage, and modifiers
    public int TotalAttack => BaseAttack + _tempDamage + (_modifiers.TryGetValue(ModifierType.Strength, out int strength) ? strength : 0) - (_modifiers.TryGetValue(ModifierType.Weakness, out int weakness) ? weakness : 0);
    #endregion

    #region Game Object References
    [SerializeField] private TMPro.TMP_Text _cardNameText;
    [SerializeField] private GameObject _descriptionContainer;
    [SerializeField] private TMPro.TMP_Text _descriptionText;
    [SerializeField] private TMPro.TMP_Text _attackValueText;
    #endregion

    public void Initialize(CardScriptable cardScriptable, Dictionary<TriggerType, List<ConditionalEffect>> conditionalEffects)
    {
        ID = cardScriptable.ID;
        Rarity = cardScriptable.Rarity;
        Name = cardScriptable.Name;
        Traits = cardScriptable.Traits;
        BaseAttack = cardScriptable.BaseAttack;
        ConditionalEffects = conditionalEffects;

        _cardNameText.text = Name;
        _descriptionText.text = cardScriptable.Description;
        _attackValueText.text = TotalAttack.ToString();
    }

    #region Interaction
    private void OnEnable()
    {
        _descriptionContainer.SetActive(false);
    }

    public void UpdateAttackValue()
    {
        _attackValueText.text = TotalAttack.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _descriptionContainer.SetActive(true);
        _descriptionContainer.transform.SetParent(GameManager.Instance.FrontDisplay, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptionContainer.transform.SetParent(transform, true);
        _descriptionContainer.SetActive(false);
    }
    #endregion

    #region Game Logic
    public void ApplyEffect(TriggerType trigger)
    {
        if (!ConditionalEffects.ContainsKey(trigger)) return;
        foreach (ConditionalEffect conditionalEffect in ConditionalEffects[trigger])
        {
            conditionalEffect.ApplyEffect();
        }
    }

    public void RevertEffect(TriggerType trigger)
    {
        if (!ConditionalEffects.ContainsKey(trigger)) return;
        foreach (ConditionalEffect conditionalEffect in ConditionalEffects[trigger])
        {
            conditionalEffect.RevertEffect();
        }
    }

    public void BindToSlot(Slot slot)
    {
        Slot = slot;
        slot.Card = this;
        transform.SetParent(slot.transform);
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0);
        }
    }

    public void UnbindFromSlot()
    {
        Slot.Card = null;
        Slot = null;
        transform.SetParent(CardManager.Instance.Graveyard.transform);
        transform.localPosition = Vector3.zero;
    }

    public void Destroy()
    {
        UnbindFromSlot();
        CardManager.Instance.RemoveCard(this);
        Destroy(gameObject);
    }

    //TODO: Implement this method
    public void TransformInto(int cardID)
    {
        CardManager.Instance.TransformCard(this, cardID);
    }

    public void ResetTemporaryState()
    {
        TempDamage = 0;
    }

    public void AddModifier(ModifierType type, int amount)
    {
        if (type == ModifierType.TempDamageUp)
        {
            TempDamage += amount;
        }
        else
        {
            if (!_modifiers.ContainsKey(type))
            {
                _modifiers[type] = amount;
            }
            else
            {
                _modifiers[type] += amount;
            }
        }
    }

    public void RemoveModifier(ModifierType type, int amount)
    {
        if (type == ModifierType.TempDamageUp)
        {
            TempDamage -= amount;
        }
        else
        {
            if (_modifiers.ContainsKey(type))
            {
                _modifiers[type] -= amount;
                if (_modifiers[type] <= 0)
                {
                    _modifiers.Remove(type);
                }
            }
        }
    }
    #endregion
}