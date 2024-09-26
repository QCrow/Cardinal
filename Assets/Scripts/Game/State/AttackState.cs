using UnityEngine;

public class AttackState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        Debug.Log("Entering Attack State...");
        Attack();
        GameManager.Instance.ChangeState(new RewardState());
    }

    public void OnExit(GameManager gameManager)
    {
    }

    private void Attack()
    {
        Debug.Log("Attacking...");
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