using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : SerializedMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Card Base Data
    public int ID;
    public RarityType Rarity;
    public string Name;
    public int Price;
    public TMPro.TMP_Text _amountInDeck;
    public int BaseAttack;
    public bool isSold = false;

    public Dictionary<TriggerType, List<ConditionalEffect>> ConditionalEffects = new();
    #endregion

    #region Card Runtime State
    public Slot Slot = null;
    [SerializeField] private Dictionary<CardModifierType, int> _permanentModifiers = new();

    [SerializeField] private Dictionary<CardModifierType, int> _temporaryModifiers = new();

    // The total attack of the card, including base attack, temporary damage, and modifiers
    public int TotalAttack => Math.Max(0, BaseAttack + GetModifierByType(CardModifierType.Strength) - GetModifierByType(CardModifierType.Weakness));
    #endregion

    #region Game Object References
    [SerializeField] private TMPro.TMP_Text _cardNameText;
    [SerializeField] private GameObject _descriptionContainer;
    [SerializeField] private TMPro.TMP_Text _descriptionText;
    [SerializeField] private TMPro.TMP_Text _attackValueText;
    [SerializeField] private GameObject _cycleContainer;
    [SerializeField] private TMPro.TMP_Text _currentCycleValueText;
    [SerializeField] private TMPro.TMP_Text _price;
    [SerializeField] private TMPro.TMP_Text SoldLabel;
    #endregion

    public void Initialize(CardScriptable cardScriptable, Dictionary<TriggerType, List<ConditionalEffect>> conditionalEffects)
    {
        ID = cardScriptable.ID;
        Rarity = cardScriptable.Rarity;
        Name = cardScriptable.Name;
        BaseAttack = cardScriptable.BaseAttack;
        ConditionalEffects = conditionalEffects;

        _cycleContainer.SetActive(false);
        if (conditionalEffects.TryGetValue(TriggerType.OnAttack, out List<ConditionalEffect> cycleEffects))
        {
            foreach (ConditionalEffect cycleEffect in cycleEffects)
            {
                if (cycleEffect.GetType() == typeof(CycleCondition))
                {
                    _cycleContainer.SetActive(true);
                }
            }
        }
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

    public void UpdateCycleValue(int currentCycle)
    {
        _currentCycleValueText.text = currentCycle.ToString();
    }

    public void UpdatePriceValue()
    {
        // Update the price value text
        _price.text = $"{Price}";

        // Move the PriceText component up slightly (adjust y value as needed)
        RectTransform priceRect = _price.GetComponent<RectTransform>();
        if (priceRect != null)
        {
            priceRect.anchoredPosition += new Vector2(0, 10);  // Move 10 units up
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _descriptionContainer.SetActive(true);
        _descriptionContainer.transform.SetParent(UIManager.Instance.OverlayDisplay, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptionContainer.transform.SetParent(transform, true);
        _descriptionContainer.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TryPurchase();
    }
    #endregion

    #region Game Logic
    private void TryPurchase()
    {
        int playerGold = ShopManager.Instance.Gold;  // Get current player gold

        if (isSold)
        {
            Debug.Log($"{Name} has already been sold.");
            return;  // Exit if the card is already sold
        }

        if (playerGold >= Price)
        {
            ShopManager.Instance.SpendGold(Price);  // Deduct gold
            Debug.Log($"Purchased {Name} for {Price} Gold.");

            // Optionally: Add card to player's deck or inventory
            CardManager.Instance.AddCardPermanently(ID);

            isSold = true;
            SoldLabel.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Not enough gold to purchase this card.");
        }
    }
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
        CardManager.Instance.RemoveCardPermanently(this);
        Destroy(gameObject);
    }

    public void TransformTemporarilyInto(int cardID)
    {
        CardManager.Instance.TransformCardTemporarily(this, cardID);
    }

    public void ResetTemporaryState()
    {
        _temporaryModifiers.Clear();
    }

    public void AddModifier(CardModifierType type, int amount, bool isPermanent)
    {
        if (isPermanent)
        {
            if (!_permanentModifiers.ContainsKey(type))
            {
                _permanentModifiers[type] = amount;
            }
            else
            {
                _permanentModifiers[type] += amount;
            }
        }
        else
        {
            if (!_temporaryModifiers.ContainsKey(type))
            {
                _temporaryModifiers[type] = amount;
            }
            else
            {
                _temporaryModifiers[type] += amount;
            }
        }
    }

    public void RemoveModifier(CardModifierType type, int amount, bool isPermanent)
    {
        if (isPermanent)
        {
            if (_permanentModifiers.ContainsKey(type))
            {
                _permanentModifiers[type] -= amount;
                if (_permanentModifiers[type] <= 0)
                {
                    _permanentModifiers.Remove(type);
                }
            }
        }
        else
        {
            if (_temporaryModifiers.ContainsKey(type))
            {
                _temporaryModifiers[type] -= amount;
                if (_temporaryModifiers[type] <= 0)
                {
                    _temporaryModifiers.Remove(type);
                }
            }
        }
    }

    public void DisableCycleContainer()
    {
        _cycleContainer.SetActive(false);
    }

    /// <summary>
    /// Get the total modifier value of a specific type
    /// </summary>
    /// <param name="type">
    /// The type of modifier to get the value of
    /// </param>
    /// <returns>
    /// The total modifier value of the specified type, 0 if no modifier of that type exists
    /// </returns>
    public int GetModifierByType(CardModifierType type)
    {
        return (_permanentModifiers.TryGetValue(type, out int value) ? value : 0) + (_temporaryModifiers.TryGetValue(type, out value) ? value : 0);
    }
    #endregion
}