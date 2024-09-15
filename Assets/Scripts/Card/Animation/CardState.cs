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

// TODO: Change this to apply to CardVisual instead of Card
public class CardHoverState : CardState
{
    private Tween _hoverTween;

    public override void OnEnter(Card card)
    {
        card.transform.DOKill();

        // Infinite loop that alternates between up and down
        _hoverTween = card.transform.DOMoveY(card.OriginalPosition.y + card.HoverAmount, card.HoverDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public override void OnExit(Card card)
    {
        _hoverTween.Kill();
    }

    public override void OnUpdate(Card card)
    {
    }
}

public class CardDraggedState : CardState
{
    public const float MaxVelocity = 50.0f; // The maximum velocity (units per second)
    private Vector2 _currentVelocity;

    public override void OnEnter(Card card)
    {
        card.transform.DOKill();
        _currentVelocity = Vector2.zero; // Initialize velocity
        GameManager.Instance.SelectedCard = card;
        card.SetRaycastTargetActive(false);
        card.SendToFront();
    }

    public override void OnExit(Card card)
    {
        card.ResetLayout();
        card.SetRaycastTargetActive(true);
    }

    public override void OnUpdate(Card card)
    {
        // Convert the card's 3D position to a 2D position (ignoring the Z axis)
        Vector2 currentPosition2D = new Vector2(card.transform.position.x, card.transform.position.y);

        // Calculate the direction to the target position in 2D
        Vector2 direction = (card.TargetPosition - currentPosition2D).normalized;

        // Move the card towards the target with a velocity cap
        Vector2 newPosition = Vector2.SmoothDamp(currentPosition2D, card.TargetPosition, ref _currentVelocity, 0.1f, MaxVelocity); //TODO: Send the velocity cap to CardVisual! In Logic, the card can move as fast as it wants

        // Update the card's transform with the new position (keep the original z-axis)
        card.transform.position = new Vector3(newPosition.x, newPosition.y, card.transform.position.z);
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