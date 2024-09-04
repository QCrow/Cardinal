using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public int Col;
    public int Row;

    [SerializeField] private Card _card;
    public Card Card { get => _card; set => _card = value; }

    public void OnDrop(PointerEventData eventData)
    {
        if (!Card)
        {
            GameObject droppedObject = eventData.pointerDrag;
            Card droppedCard = droppedObject.GetComponent<Card>();
            if (droppedCard.Slot != null)
            {
                return;
            }
            droppedCard.Slot = this;
            Card = droppedCard;
            EffectResolveManager.Instance.ResolveOnPlayEffects(droppedCard);
        }
    }
}
