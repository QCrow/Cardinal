using UnityEngine;
using DG.Tweening;

public abstract class CardState
{
    public abstract void OnEnter(Card card);
    public abstract void OnExit(Card card);
    public abstract void OnUpdate(Card card);
}

public class CardIdleState : CardState
{
    public override void OnEnter(Card card)
    {
        card.transform.DOKill();
        card.transform.DOMove(card.OriginalPosition, 0.5f)
            .OnStart(() => card.IsMovingBack = true)
            .OnComplete(() =>
            {
                card.IsMovingBack = false;

                // Check if the mouse is hovering over the card
                if (RectTransformUtility.RectangleContainsScreenPoint(card.GetComponent<RectTransform>(), Input.mousePosition, card.UICamera))
                {
                    card.ChangeState(new CardHoverState());
                }
            });
    }

    public override void OnExit(Card card)
    {
    }

    public override void OnUpdate(Card card)
    {
    }
}

public class CardHoverState : CardState
{
    public override void OnEnter(Card card)
    {
        // Quick overshoot on scaling for a punchy feel
        card.CardVisual.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.1f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
                card.CardVisual.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f)
            );

        // Adding a slight shake effect for position and rotation
        card.CardVisual.transform.DOShakePosition(0.1f, new Vector3(10f, 10f, 0), 10, 90, false, true)
            .SetEase(Ease.InOutSine);

        card.CardVisual.transform.DOShakeRotation(0.1f, new Vector3(0, 0, 15f), 10, 90)
            .SetEase(Ease.InOutSine);
    }

    public override void OnExit(Card card)
    {
        card.CardVisual.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f);
    }

    public override void OnUpdate(Card card)
    {
    }
}




public class CardDraggedState : CardState
{
    public override void OnEnter(Card card)
    {
        GameManager.Instance.SelectedCard = card;
        card.SetRaycastTargetActive(false);
        card.SendToFront();
    }

    public override void OnExit(Card card)
    {
        card.SetRaycastTargetActive(true);
    }

    public override void OnUpdate(Card card)
    {

    }
}

public class CardInSlotState : CardState
{
    public override void OnEnter(Card card)
    {
        card.transform.DOKill();
        // card.transform.DOMove(card.OriginalPosition, 0.5f).SetEase(Ease.OutQuad);
    }

    public override void OnExit(Card card)
    {
    }

    public override void OnUpdate(Card card)
    {
    }
}

public class CardInRewardState : CardState
{
    public override void OnEnter(Card card)
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit(Card card)
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate(Card card)
    {
        throw new System.NotImplementedException();
    }
}