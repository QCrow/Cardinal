using UnityEngine;
using DG.Tweening;

public abstract class SlotContent : MonoBehaviour
{
    public Slot CurrentSlot { get; protected set; }

    protected virtual void Awake()
    {
        if (TryGetComponent<Canvas>(out var canvas))
        {
            // If the object already has a Canvas, adjust settings if needed
            canvas.overrideSorting = false; // Let the parent Canvas control sorting
        }
        else
        {
            // Do not add a Canvas unless absolutely required
            // Remove this block if you don't need the Canvas
            canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = false;
        }

        if (TryGetComponent<RectTransform>(out var rectTransform))
        {
            rectTransform.localScale = Vector3.one;
        }
    }


    /// <summary>
    /// Binds the content to a specified slot.
    /// </summary>
    /// <param name="slot">The slot to bind to.</param>
    public virtual void BindToSlot(Slot slot)
    {
        if (slot == null) return;

        if (slot.Content != null)
        {
            slot.Content.UnbindFromSlot();
        }

        CurrentSlot = slot;
        slot.Content = this;
        transform.SetParent(slot.ContentContainer.transform);
        ResetTransform();
    }

    /// <summary>
    /// Unbinds the content from its current slot.
    /// </summary>
    public virtual void UnbindFromSlot()
    {
        if (CurrentSlot != null)
        {
            CurrentSlot.Content = null;
            CurrentSlot = null;
        }
    }

    /// <summary>
    /// Moves the content to the graveyard.
    /// </summary>
    public void MoveToGraveyard()
    {
        transform.SetParent(CardSystem.Instance.Graveyard.transform);
        ResetTransform();
    }

    public void MoveToAndSetParent(RectTransform dst, System.Action callback = null)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.SetParent(dst);
        rectTransform.DOAnchorPos(Vector2.zero, 0.5f).OnComplete(() =>
        {
            ResetTransform();
            callback?.Invoke();
        });
    }

    /// <summary>
    /// Resets the content's transform to default values.
    /// </summary>
    public void ResetTransform()
    {
        if (TryGetComponent<RectTransform>(out var rectTransform))
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0);
        }
    }
}