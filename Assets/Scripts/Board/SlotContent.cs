using UnityEngine;

public class SlotContent : MonoBehaviour
{
    public Slot CurrentSlot { get; protected set; }

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

        transform.SetParent(CardSystem.Instance.Graveyard.transform);
        ResetTransform();
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