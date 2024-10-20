using System.Collections.Generic;

public class RewardPhase : IBattlePhase
{
    public void OnEnter()
    {
        UIManager.Instance.SetRewardsPanelActive(true);
        List<Reward> rewards = CardManager.Instance.GenerateRewardChoices();
        UIManager.Instance.SetRewards(rewards);
    }

    public void OnExit()
    {
    }
}