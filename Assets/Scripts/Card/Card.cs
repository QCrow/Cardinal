using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private int _cardID;
    public int CardID { get => _cardID; protected set => _cardID = value; }

    [SerializeField] protected CardVisual _cardVisual;

    public bool IsHovering;
    public bool IsDragging;
    public bool WasDragged;
    [SerializeField] private Vector2 _offset;

    private Vector3 _initialPosition;
    private Transform _initialParent;

    //? This might not be needed in the future, right now is kept in for early dev
    [HideInInspector] public Slot Slot;

    private void Start()
    {
        _initialParent = transform.parent;
    }

    private void Update()
    {
        if (IsDragging)
        {
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent as RectTransform,
                Input.mousePosition,
                GameManager.Instance.UICamera,
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
            GameManager.Instance.UICamera,
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
