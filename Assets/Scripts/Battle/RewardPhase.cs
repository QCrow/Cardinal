using System.Collections.Generic;

public class RewardPhase : IBattlePhase
{
    public void OnEnter()
    {
        UIManager.Instance.SetRewardsPanelActive(true);
        List<CardReward> rewards = CardSystem.Instance.CardRewardGenerator.GenerateCardRewardChoices(0, 3);
        UIManager.Instance.SetRewards(rewards);

        Board.Instance.DeployedCards.ForEach(card => card.ResetCardModifierState(ModifierPersistenceType.Battle));
    }

    public void OnExit()
    {
        UIManager.Instance.SetRewardsPanelActive(false);
    }
}