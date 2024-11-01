public class Reward
{
    public int RewardID;
    public string RewardName;
    public string RewardText;
    public int RewardAttack;
    public int RewardCycle;

    public Reward(int rewardID, string rewardName, string rewardText, int rewardAtack)
    {
        RewardID = rewardID;
        RewardName = rewardName;
        RewardText = rewardText;
        RewardAttack = rewardAtack;
        RewardCycle = 0;
    }
}