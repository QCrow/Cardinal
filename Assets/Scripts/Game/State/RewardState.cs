using System.Collections.Generic;

public class RewardState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        List<Reward> rewards = CardManager.Instance.GenerateRewardChoices();
        UIManager.Instance.SetRewards(rewards);
    }

    public void OnExit(GameManager gameManager)
    {
    }
}