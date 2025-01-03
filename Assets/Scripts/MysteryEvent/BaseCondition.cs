using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseCondition
{
    // Common fields or functions can go here if needed.
    public abstract bool Evaluate();
}

[System.Serializable]
public class GoldRequirementCondition : BaseCondition
{
    public int requiredGold;

    public override bool Evaluate()
    {
        return ShopManager.Instance.Gold >= requiredGold;
    }
}

[System.Serializable]
public class SpecificCardRequirementCondition : BaseCondition
{
    public int cardId;
    public int requiredCount;

    public override bool Evaluate()
    {
        int count = CardManager.Instance.GetGroupedDeck()
                                       .TryGetValue(cardId, out var cardCount)
                                       ? cardCount
                                       : 0;
        return count >= requiredCount;
    }
}

[System.Serializable]
public class NoCondition : BaseCondition
{
    public override bool Evaluate()
    {
        return true;
    }
}




