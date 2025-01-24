public class CardReward
{
    public int RewardID;
    public string RewardName;
    public string RewardText;

    public CardReward(int rewardID)
    {
        AbstractCard cardTemplate = CardLibrary.GetCard(rewardID);

        RewardID = rewardID;
        // RewardName = LocalizationHandler.Instance.FetchCardName(rewardID);
        // RewardText = LocalizationHandler.Instance.GetCardDescription(rewardID);
        // RewardText = LocalizationHandler.Instance.ParseDynamicDescription(RewardText, cardTemplate);
    }
}