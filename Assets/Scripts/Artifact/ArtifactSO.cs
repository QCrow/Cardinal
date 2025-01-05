using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifact", menuName = "Artifacts/Artifact")]
public class ArtifactSO : ScriptableObject
{
    [Header("Basic Info")]
    public int artifactID;
    public string artifactName;
    [TextArea]
    public string description;

    [Header("Effects")]
    // Using SerializeReference for polymorphic list support (Unity 2020+)
    [SerializeReference]
    public List<BaseEffect> effects = new List<BaseEffect>();

    /// <summary>
    /// Applies all effects that match the given trigger.
    /// </summary>
    public void ActivateEffect(TriggerCondition trigger)
    {
        foreach (var effect in effects)
        {
            if (effect.triggerCondition == trigger)
            {
                effect.Activate();
            }
        }
    }

    public void DeactivateAllEffects()
    {
        foreach (var effect in effects)
        {
            effect.Deactivate();
        }
    }
}


public enum TriggerCondition
{
    OnObtain,
    OnTurnStart,
    OnDeploy,
    // etc.
}


[System.Serializable]
public abstract class BaseEffect
{
    [Header("Trigger Condition")]
    public TriggerCondition triggerCondition;

    /// <summary>
    /// The main method for applying the effect logic.
    /// </summary>
    public abstract void Activate();

    public abstract void Deactivate();
}



[System.Serializable]
public class GainGoldEffect : BaseEffect
{
    public int goldAmount;

    public override void Activate()
    {
        PlayerManager.Instance.IncreaseGold(goldAmount);
        Debug.Log($"[GainGoldEffect] Gained {goldAmount} gold.");
    }

    public override void Deactivate()
    {
        
    }
}

[System.Serializable]
public class LoseHealthEffect : BaseEffect
{
    public int healthAmount;

    public override void Activate()
    {
        PlayerManager.Instance.DecreaseHealth(healthAmount);
        Debug.Log($"[LoseHealthEffect] Lost {healthAmount} health.");
    }

    public override void Deactivate()
    {

    }
}

[System.Serializable]
public class ChangeMovePerTurnEffect : BaseEffect
{
    public int count;

    public override void Activate()
    {
        BattleManager.Instance.ChangeMoveCount(count);
        Debug.Log($"[ChangeMovePerTurnEffect] Move count changed by {count}.");
    }

    public override void Deactivate()
    {
        // Revert the effect: if we decreased by 1, now we add 1 back
        BattleManager.Instance.ChangeMoveCount(-count);
        Debug.Log($"[ChangeMovePerTurnEffect] Move count changed by {-count} (DEACTIVATE).");
    }
}

[System.Serializable]
public class UnlockPlotEffect : BaseEffect
{
    //TODO: have a variable flag id and manager the flags
    public override void Activate()
    {
        PlayerManager.Instance.hasUnlockedStoryBranch = true;
        Debug.Log("[UnlockPlotEffect] Plot branch unlocked!");
    }

    public override void Deactivate()
    {
        PlayerManager.Instance.hasUnlockedStoryBranch = false;
    }
}



