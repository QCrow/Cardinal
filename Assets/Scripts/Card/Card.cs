using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

public abstract class Card : SerializedMonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Logic related variables
    public int CardID;
    public string CardName;

    // Effect using resolve trigger as key for quick access
    public Dictionary<CardEffectTriggerType, List<CardCondition>> ConditionalEffects = new();
    public List<string> ValidTargets = new(); // TODO: Make this into enum

    public Slot Slot;
    public Dictionary<ModifierType, Modifier> Modifiers = new();
    #endregion


    #region Movement/Animation related variables
    public CardState CurrentState { get; private set; }

    [HideInInspector] public CardVisual CardVisual;
    public Camera UICamera;
    public GameObject AnchorContainer;

    public Vector3 OriginalPosition;

    public bool IsMovingBack = false;

    protected bool _canInteract => InputManager.Instance.CanProcessGameInput() && !IsMovingBack;
    #endregion

    public void ChangeState(CardState newState)
    {
        CurrentState?.OnExit(this);

        CurrentState = newState;

        CurrentState?.OnEnter(this);
    }

    private void Start()
    {
        UICamera = GetComponentInParent<Canvas>().worldCamera;
        // _layoutElement = GetComponent<LayoutElement>();

        StartCoroutine(WaitForEndOfFrame());

        IEnumerator WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            OriginalPosition = transform.position;
            ChangeState(new CardIdleState());
        }
    }

    private void Update()
    {
        if (CurrentState is not CardDraggedState)
        {
            RectTransform rect = GetComponent<RectTransform>();
            rect.offsetMin = Vector2.zero;  // This sets the left and bottom to 0
            rect.offsetMax = Vector2.zero;  // This sets the right and top to 0
        }
        if (!_canInteract) return;

        CurrentState?.OnUpdate(this);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canInteract) return;
        if (CurrentState is not CardHoverState) return;
        GameManager.Instance.SelectedCard = this;
        ChangeState(new CardDraggedState());
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!_canInteract) return;

        RectTransform rectTransform = transform as RectTransform;
        RectTransform parentRectTransform = transform.parent as RectTransform;

        // Get the current mouse position in world space
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            parentRectTransform,
            Input.mousePosition,
            UICamera,
            out Vector3 worldMousePosition);

        // Calculate the offset in local space (pivot offset from the center of the RectTransform)
        Vector2 localPivotOffset = new Vector2(
            rectTransform.rect.width * (rectTransform.pivot.x - 0.5f),
            rectTransform.rect.height * (0.5f - rectTransform.pivot.y) //! The y calculation might be wrong, but is not used for now
        );

        // Convert local space pivot offset to world space
        Vector3 worldPivotOffset = parentRectTransform.TransformPoint(localPivotOffset) - parentRectTransform.TransformPoint(Vector2.zero);

        // Adjust the world position to account for the pivot offset
        transform.position = worldMousePosition + worldPivotOffset;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!_canInteract) return;

        //TODO: Change the condition to check if the card is placeable with the tile
        if (!Slot)
        {
            ChangeState(new CardIdleState());
        }
        else
        {
            ChangeState(new CardInSlotState());
            TransformUtil.MoveToAndSetParent(gameObject, Slot.gameObject);
            TransformUtil.MoveToAndSetParent(CardVisual.gameObject, Slot.gameObject);
            Hand.Instance.RemoveCard(gameObject);
        }

        GameManager.Instance.SelectedCard = null;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!_canInteract) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (Slot) Slot.Card = null;
            ForceRemove();
        }
    }

    // zoom in the card if the mouse hover on the card
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!_canInteract) return;
        if (GameManager.Instance.SelectedCard) return;

        if (CurrentState == null || CurrentState is not CardIdleState) return;
        OriginalPosition = transform.position;
        ChangeState(new CardHoverState());
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!_canInteract) return;
        if (GameManager.Instance.SelectedCard) return;

        if (CurrentState == null || CurrentState is not CardHoverState) return;
        ChangeState(new CardIdleState());
    }


    public void Remove()
    {
        if (Modifiers.ContainsKey(ModifierType.Everlasting) || Slot.Modifiers.ContainsKey(ModifierType.Everlasting)) return;
        EffectResolveManager.Instance.ResolveOnRemoveEffects(this);
        if (Slot) Slot.Card = null;
        Destroy(CardVisual.gameObject);
        Destroy(gameObject);
    }

    //Forcefully remove the card without checking its modifier type, shall only be use for development purpose
    public void ForceRemove()
    {
        EffectResolveManager.Instance.ResolveOnRemoveEffects(this);
        if (Slot) Slot.Card = null;
        Destroy(CardVisual.gameObject);
        Destroy(gameObject);
    }

    public void AddConditionalEffect(CardEffectTriggerType triggerType, CardCondition condition)
    {
        if (!ConditionalEffects.ContainsKey(triggerType))
        {
            ConditionalEffects[triggerType] = new List<CardCondition>();
        }
        ConditionalEffects[triggerType].Add(condition);
    }

    public void AddModifier(ModifierType modifierType, int amount)
    {
        if (!Modifiers.ContainsKey(modifierType))
        {
            Modifiers[modifierType] = ModifierFactory.CreateModifier(modifierType, amount);
        }
        Modifiers[modifierType].Amount += amount;
    }

    public void RemoveModifier(ModifierType modifierType, int amount)
    {
        if (Modifiers.ContainsKey(modifierType))
        {
            Modifiers[modifierType].Amount += amount;
            if (Modifiers[modifierType].Amount <= 0)
            {
                Modifiers.Remove(modifierType);
            }
        }
    }

    public void SetRaycastTargetActive(bool isActive)
    {
        Image i = GetComponent<Image>();
        i.raycastTarget = isActive;
    }

    public void SendToFront()
    {
        CardVisual.transform.SetAsLastSibling();
    }
}