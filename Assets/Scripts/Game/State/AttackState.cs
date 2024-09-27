using UnityEngine;

public class AttackState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        Attack();
        GameManager.Instance.ChangeState(new RewardState());
    }

    public void OnExit(GameManager gameManager)
    {
    }

    private void Attack()
    {
        Board.Instance.DeployedCards.ForEach(card =>
        {
            if (card.Trigger == TriggerType.OnAttack)
            {
                card.ApplyEffect();
            }

            GameManager.Instance.InflictDamage(card.TotalAttack);
        });
    }
}