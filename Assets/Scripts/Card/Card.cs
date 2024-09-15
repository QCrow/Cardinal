using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;

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
    protected CardState _currentState;

    [SerializeField] protected CardVisual _cardVisual;
    public Camera UICamera;

    protected LayoutElement _layoutElement;
    protected int _layoutOrder;

    //Hover Variable
    public readonly float HoverAmount = 0.1f;
    public readonly float HoverDuration = 0.5f;
    public readonly Vector3 ZoomVector = new Vector3(1.0f, 1.0f, 1.0f);

    public Vector3 OriginalPosition;
    public Vector2 TargetPosition;

    public bool IsMovingBack = false;

    protected bool _canInteract => InputManager.Instance.CanProcessGameInput() && !IsMovingBack;
    #endregion

    public void ChangeState(CardState newState)
    {
        _currentState?.OnExit(this);

        _currentState = newState;

        _currentState?.OnEnter(this);
    }

    private void Start()
    {
        UICamera = GetComponentInParent<Canvas>().worldCamera;
        _layoutElement = GetComponent<LayoutElement>();

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
        if (!_canInteract) return;

        _currentState?.OnUpdate(this);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canInteract) return;
        if (_currentState is not CardHoverState) return;
        GameManager.Instance.SelectedCard = this;
        ChangeState(new CardDraggedState());
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!_canInteract) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            transform.parent as RectTransform,
            Input.mousePosition,
            UICamera,
            out Vector3 worldMousePosition);

        TargetPosition = worldMousePosition;
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
            TransformUtil.MoveToAndSetParent(gameObject, Slot.gameObject);

            Hand.Instance.RemoveCard(gameObject);
            ChangeState(new CardInSlotState());
        }

        GameManager.Instance.SelectedCard = null;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!_canInteract) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // EffectResolveManager.Instance.ResolveOnRemoveEffects(this);
            if (Slot) Slot.Card = null;
            Destroy(gameObject);
        }
    }

    // zoom in the card if the mouse hover on the card
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!_canInteract) return;

        if (_currentState == null || _currentState is not CardIdleState) return;
        OriginalPosition = transform.position;
        ChangeState(new CardHoverState());
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!_canInteract) return;

        if (_currentState == null || _currentState is not CardHoverState) return;
        ChangeState(new CardIdleState());
    }


    public void Remove()
    {
        if (Modifiers.ContainsKey(ModifierType.Everlasting) || Slot.Modifiers.ContainsKey(ModifierType.Everlasting)) return;
        EffectResolveManager.Instance.ResolveOnRemoveEffects(this);
        if (Slot) Slot.Card = null;
        Destroy(gameObject);
    }

    //Forcefully remove the card without checking its modifier type, shall only be use for development purpose
    public void ForceRemove()
    {
        EffectResolveManager.Instance.ResolveOnRemoveEffects(this);
        if (Slot) Slot.Card = null;
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
        _layoutOrder = transform.GetSiblingIndex();
        _layoutElement.ignoreLayout = true;
        transform.SetAsLastSibling();
    }

    public void ResetLayout()
    {
        transform.SetSiblingIndex(_layoutOrder);
        _layoutElement.ignoreLayout = false;
    }
}