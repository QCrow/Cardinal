using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class Slot : SerializedMonoBehaviour, IDropHandler
{
    public int Col;
    public int Row;

    [SerializeField] private Card _card;
    public Card Card { get => _card; set => _card = value; }

    public Dictionary<ModifierType, Modifier> Modifiers = new();
    public List<Slot> Neighbors => Board.Instance.GetNeighbors(this);

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

    public void AddModifier(ModifierType modifierType, int amount)
    {
        // Debug.Log($"Adding modifier {amount}");
        if (!Modifiers.ContainsKey(modifierType))
        {
            Modifiers[modifierType] = ModifierFactory.CreateModifier(modifierType, amount);
        }
        else
        {
            Modifiers[modifierType].Amount += amount;
        }
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