public class CardReward
{
    public int RewardID;
    public string RewardName;
    public string RewardText;
    public int RewardAttack;
    public int RewardCycle;

    public CardReward(int rewardID, string rewardName, string rewardText, int rewardAtack, int rewardCycle)
    {
        RewardID = rewardID;
        RewardName = rewardName;
        RewardText = rewardText;
        RewardAttack = rewardAtack;
        RewardCycle = rewardCycle;
    }

    public CardReward(CardScriptable card)
    {
        RewardID = card.ID;
        RewardName = card.Name;
        RewardText = card.Description;
        RewardAttack = card.BaseAttack;

        if (card.Conditions.Find(x => x.Condition == ConditionType.Cycle) != null)
        {
            RewardCycle = card.Conditions.Find(x => x.Condition == ConditionType.Cycle).CycleCount;
        }
        else
        {
            RewardCycle = 0;
        }
    }
}