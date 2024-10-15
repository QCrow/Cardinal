using System.Linq;
using UnityEngine;

public class ControlState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        if (gameManager.CanDeploy())
        {
            UIManager.Instance.DeployButton.onClick.AddListener(OnDeployButtonPressed);
        }
        else
        {
            UIManager.Instance.DeployButton.interactable = false;
        }
        UIManager.Instance.AttackButton.onClick.AddListener(OnAttackButtonPressed);
        GameManager.Instance.RemainingMoveCount = GameManager.Instance.MovePerTurn;
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
        Board.Instance.ApplyMovement(direction, index, magnitude);
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
        UIManager.Instance.RefreshAttackValue();
    }

    private void RevertWhileInPlayEffects()
    {
        foreach (Card card in Board.Instance.DeployedCards)
        {
            card.RevertEffect(TriggerType.WhileInPlay);
        }
    }
}