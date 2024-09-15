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
    // TODO: Make some of these variables into private field
    public int CardID;
    public string CardName;

    // Effect using resolve trigger as key for quick access
    public Dictionary<CardEffectTriggerType, List<CardCondition>> ConditionalEffects = new();
    public List<string> ValidTargets = new(); // TODO: Make this into enum

    public Slot Slot;
    public Dictionary<ModifierType, Modifier> Modifiers = new();
    #endregion

    // TODO: Might need to refactor visual to make it cleanly separate from logic
    #region Visual related variables
    [SerializeField] protected CardVisual _cardVisual;
    private Camera _uiCamera;

    public bool IsHovering;
    public bool IsDragging;
    public bool WasDragged;
    [SerializeField] private Vector2 _offset;

    private Vector3 _initialPosition;
    private Transform _initialParent;

    public float hoverAmount = 0.1f;
    private Vector3 zoomVector;
    private Vector3 OriginalY;

    //cardHover Variable
    private Vector3 _originalPosition;
    public float hoverDuration = 0.5f;
    private Tween hoverTween;

    #endregion


    private void Start()
    {
        _initialParent = transform.parent;
        _uiCamera = GetComponentInParent<Canvas>().worldCamera;
        _originalPosition = transform.position;
    }

    private void Update()
    {
        if (!InputManager.Instance.CanProcessGameInput())
        {
            return;
        }

        if (IsDragging)
        {
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent as RectTransform,
                Input.mousePosition,
                _uiCamera,
                out mousePosition);

            Vector2 targetPosition = mousePosition + _offset;

            transform.localPosition = targetPosition;
        }
    }


    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!InputManager.Instance.CanProcessGameInput())
        {
            return;
        }
        //? Might be changed in the future
        if (Slot) return;

        GameManager.Instance.SelectedCard = this;
        _initialPosition = transform.localPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            eventData.position,
            _uiCamera,
            out _offset);

        _offset = (Vector2)transform.localPosition - _offset;

        // //? Test to see if it works
        Image i = GetComponent<Image>();
        i.raycastTarget = false;

        IsDragging = true;
        WasDragged = true;

    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!InputManager.Instance.CanProcessGameInput())
        {
            return;
        }
        //zoom out the card
        zoomVector = new Vector3(0.7f, 0.7f, 0.7f);
        transform.localScale = zoomVector;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!InputManager.Instance.CanProcessGameInput())
        {
            return;
        }

        IsDragging = false;
        if (!Slot)
        {
            //TODO: Refactor this into TransformUtil
            transform.DOMove(_initialParent.TransformPoint(_initialPosition), 0.5f)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() =>
                     {
                         transform.SetParent(_initialParent);
                         transform.localPosition = _initialPosition; // Ensure the final position is accurate
                     });
        }
        else
        {
            //TODO: Change the condition to check if the card is placeable with the tile
            TransformUtil.MoveToAndSetParent(gameObject, Slot.gameObject);

            Hand.Instance.RemoveCard(gameObject);
        }
        GameManager.Instance.SelectedCard = null;
        StartCoroutine(WaitForEndOfFrame());

        IEnumerator WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            WasDragged = false;
            Image i = GetComponent<Image>();
            i.raycastTarget = true;
        }

        //zoom in the card
        zoomVector = new Vector3(1.0f, 1.0f, 1.0f);
        transform.localScale = zoomVector;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!InputManager.Instance.CanProcessGameInput())
        {
            return;
        }
        
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
        if (IsHovering) return;  
        IsHovering = true;
        transform.DOKill();

        zoomVector = new Vector3(1.0f, 1.0f, 1.0f);
        transform.localScale += zoomVector * hoverAmount;
        // Infinite loop that alternates between up and down
        hoverTween = transform.DOMoveY(_originalPosition.y + hoverAmount, hoverDuration)
            .SetLoops(-1, LoopType.Yoyo) 
            .SetEase(Ease.InOutSine);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!IsHovering) return;
        IsHovering = false;
        // Stop the hover animation
        hoverTween.Kill();

        zoomVector = new Vector3(1.0f, 1.0f, 1.0f);
        transform.localScale -= zoomVector * hoverAmount;
        transform.DOMoveY(_originalPosition.y, hoverDuration).SetEase(Ease.OutQuad);
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
}