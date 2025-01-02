using System;
using UnityEngine;

[Serializable]
public abstract class BaseOutcome
{
    [TextArea] public string outcomeDescription;  // Short explanation or flavor text
    public abstract void ApplyOutcome();
}

[Serializable]
public class GainGoldOutcome : BaseOutcome
{
    public int goldAmount;

    public override void ApplyOutcome()
    {
        ShopManager.Instance.Gold += goldAmount;
    }
}

// 2) Lose Gold
[Serializable]
public class LoseGoldOutcome : BaseOutcome
{
    public int goldAmount;

    public override void ApplyOutcome()
    {
        ShopManager.Instance.Gold = Mathf.Max(0, ShopManager.Instance.Gold - goldAmount);
    }
}

// 3) Gain Card
[Serializable]
public class GainCardOutcome : BaseOutcome
{
    public int cardId;
    public int quantity;

    public override void ApplyOutcome()
    {
        for (int i = 0; i < quantity; i++)
        {
            CardManager.Instance.AddCardPermanently(cardId);
        }
    }
}