using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public abstract class Card : SerializedMonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    #region Logic related variables
    // TODO: Make some of these variables into private field
    public int CardID;
    public string CardName;

    // Effect using resolve trigger as key for quick access
    public Dictionary<CardEffectTriggerType, List<CardCondition>> ConditionalEffects = new();
    public List<string> ValidTargets = new(); // TODO: Make this into enum

    [HideInInspector] public Slot Slot;
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
    #endregion


    private void Start()
    {
        _initialParent = transform.parent;
        _uiCamera = GetComponentInParent<Canvas>().worldCamera;
    }

    private void Update()
    {
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
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
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
            EffectResolveManager.Instance.ResolveOnPlayEffects(this);
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
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right clicked on card");
        }
    }
}


