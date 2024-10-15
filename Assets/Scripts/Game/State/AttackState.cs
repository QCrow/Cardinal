using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;

public class AttackState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        Attack();
        gameManager.DecrementAttackCounter();
        gameManager.ResetDeployCount();
        UIManager.Instance.UpdateDeployCounter(GameManager.Instance.RemainingDeployCount);

        if (gameManager._remainingAttacks == 0 && gameManager.CurrentHealth > 0)
        {
            GameManager.Instance.ChangeState(new LossState());
        }
        else if(gameManager.CurrentHealth <= 0)
        {
            GameManager.Instance.ChangeState(new RewardState());
        }
        else
        {
            GameManager.Instance.ChangeState(new WaitState());
        }
    }

    public void OnExit(GameManager gameManager)
    {
        UIManager.Instance.RefreshAttackValue();
    }

    private void Attack()
    {
        Board.Instance.DeployedCards.ForEach(card =>
        {
            card.ApplyEffect(TriggerType.OnAttack);

            GameManager.Instance.InflictDamage(card.TotalAttack);
        });
    }

}