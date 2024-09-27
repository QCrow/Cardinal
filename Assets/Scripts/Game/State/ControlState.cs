using System.Linq;
using UnityEngine;

public class ControlState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        UIManager.Instance.DeployButton.onClick.AddListener(OnDeployButtonPressed);
        UIManager.Instance.AttackButton.onClick.AddListener(OnAttackButtonPressed);
        ApplyWhileInPlayEffects();
    }

    public void OnExit(GameManager gameManager)
    {
        UIManager.Instance.DeployButton.onClick.RemoveListener(OnDeployButtonPressed);
        UIManager.Instance.AttackButton.onClick.RemoveListener(OnAttackButtonPressed);
    }

    private void OnDeployButtonPressed()
    {
        GameManager.Instance.ChangeState(new DeployState());
    }

    private void OnAttackButtonPressed()
    {
        GameManager.Instance.ChangeState(new AttackState());
    }

    public void ApplyMovement(Direction direction, int index, int magnitude)
    {
        RevertWhileInPlayEffects();
        switch (direction)
        {
            case Direction.Up:
                Board.Instance.ShiftCardsOnColumn(index, -magnitude);
                break;
            case Direction.Down:
                Board.Instance.ShiftCardsOnColumn(index, magnitude);
                break;
            case Direction.Left:
                Board.Instance.ShiftCardsOnRow(index, -magnitude);
                break;
            case Direction.Right:
                Board.Instance.ShiftCardsOnRow(index, magnitude);
                break;
            case Direction.Clockwise:
                Board.Instance.RotateCardsClockwise();
                break;
            case Direction.CounterClockwise:
                Board.Instance.RotateCardsCounterClockwise();
                break;
        }
        ApplyWhileInPlayEffects();
    }

    private void ApplyWhileInPlayEffects()
    {
        foreach (Card card in Board.Instance.DeployedCards)
        {
            card.ApplyEffect(TriggerType.WhileInPlay);
        }

        // DEBUG
        UIManager.Instance.SetTotalAttack(Board.Instance.DeployedCards.Sum(card => card.TotalAttack));

    }

    private void RevertWhileInPlayEffects()
    {
        foreach (Card card in Board.Instance.DeployedCards)
        {
            card.RevertEffect(TriggerType.WhileInPlay);
        }
    }
}