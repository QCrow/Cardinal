using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardView : SlotContent
{
    private CardInstance _instance;

    #region UI References
    [BoxGroup("UI References")]
    [SerializeField] private TMP_Text _cardNameText;

    [BoxGroup("UI References")]
    [SerializeField] private GameObject _descriptionContainer;

    [BoxGroup("UI References")]
    [SerializeField] private TMP_Text _descriptionText;
    #endregion

    public void Initialize(CardInstance instance)
    {
        _instance = instance;
        SetUpUI();
    }

    public override void BindToSlot(Slot slot)
    {
        base.BindToSlot(slot);
        _instance.CurrentSlot = slot;
    }

    public override void UnbindFromSlot()
    {
        base.UnbindFromSlot();
        _instance.CurrentSlot = null;
    }

    private void OnEnable()
    {
        HideDescription();
    }

    private void OnDestroy()
    {
        UnbindFromSlot();
    }

    private void ShowDescription()
    {
        _descriptionContainer.SetActive(true);
    }

    private void HideDescription()
    {
        _descriptionContainer.SetActive(false);
    }

    private void SetUpUI()
    {
        EnsureDescriptionCanvas();
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

    // #endregion

    // #region Event Handlers

    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     ShowDescription();
    // }

    // public void OnPointerExit(PointerEventData eventData)
    // {
    //     HideDescription();
    // }


    // #endregion

    // #region Slot Interaction

    // public override void BindToSlot(Slot slot)
    // {
    //     base.BindToSlot(slot);
    //     // TODO: Implement UI update after slot binding
    // }

    // #endregion

    // #region Serialization
    // // public CardSaveData GetSaveData()
    // // {
    // //     CardSaveData saveData = new CardSaveData
    // //     (
    // //         this.gameObject.GetInstanceID(),
    // //         CurrentSlot != null ? CurrentSlot.Row : -1,
    // //         CurrentSlot != null ? CurrentSlot.Col : -1,
    // //         ID,
    // //         _conditionalEffects.SelectMany(effect => effect.Value).OfType<CycleCondition>().FirstOrDefault()?.CycleValue ?? 0,
    // //         _modifiers.ToDictionary(pair => pair.Key, pair => pair.Value.ToDictionary(pair => pair.Key, pair => pair.Value))
    // //     );

    // //     return saveData;
    // // }

    // // public void LoadFromSaveData(CardSaveData saveData)
    // // {
    // //     ID = saveData.ID;
    // //     _modifiers = saveData.Modifiers;
    // //     if (_conditionalEffects.SelectMany(effect => effect.Value).OfType<CycleCondition>().FirstOrDefault() is CycleCondition cycleCondition)
    // //     {
    // //         cycleCondition.CycleValue = saveData.CycleValue;
    // //     }
    // //     UpdateAttackValue();
    // //     UpdateCycleValue(saveData.CycleValue);
    // // }
    // #endregion
}
