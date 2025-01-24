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
        return PlayerManager.Instance.CurrentGold >= requiredGold;
    }
}

[System.Serializable]
public class SpecificCardRequirementCondition : BaseCondition
{
    public int cardId;

    public override bool Evaluate()
    {
        return PlayerManager.Instance.Decks.HasCard(cardId);
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




