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
        // Use PlayerManager to increase gold
        PlayerManager.Instance.IncreaseGold(goldAmount);
    }
}

[Serializable]
public class LoseGoldOutcome : BaseOutcome
{
    public int goldAmount;

    public override void ApplyOutcome()
    {
        // Use PlayerManager to decrease gold
        PlayerManager.Instance.DecreaseGold(goldAmount);
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
            PlayerManager.Instance.Decks.AddCard(cardId, true);
        }
    }
}

[Serializable]
public class LoadEventOutcome : BaseOutcome
{
    public int eventId; // The ID of the event to load

    public override void ApplyOutcome()
    {
        // Use the LoadEventById method from the MysteryEventManager
        MysteryEventManager.Instance.LoadEventById(eventId);
    }
}



[Serializable]
public class NoOutcome : BaseOutcome
{
    public override void ApplyOutcome()
    {
        // Does nothing
    }
}

[Serializable]
public class GainHPOutcome : BaseOutcome
{
    public int healthAmount;

    public override void ApplyOutcome()
    {
        // Use PlayerManager to increase health
        PlayerManager.Instance.IncreaseHealth(healthAmount);
    }
}

[Serializable]
public class LoseHPOutcome : BaseOutcome
{
    public int healthAmount;

    public override void ApplyOutcome()
    {
        // Use PlayerManager to decrease health
        PlayerManager.Instance.DecreaseHealth(healthAmount);
    }
}

[Serializable]
public class GainArtifactOutcome : BaseOutcome
{
    public int artifactID; // The ID of the artifact to add

    public override void ApplyOutcome()
    {
        // Assumes PlayerManager (or ArtifactManager) has a method to add an artifact by ID
        ArtifactManager.Instance.ObtainArtifactById(artifactID);
    }
}

[Serializable]
public class LoseArtifactOutcome : BaseOutcome
{
    public int artifactID; // The ID of the artifact to remove

    public override void ApplyOutcome()
    {
        // Assumes PlayerManager (or ArtifactManager) has a method to remove an artifact by ID
        ArtifactManager.Instance.RemoveArtifactById(artifactID);
    }
}

[Serializable]
public class UnlockEventOutcome : BaseOutcome
{
    public int eventId; // The ID of the event to unlock

    public override void ApplyOutcome()
    {
        if (!MysteryEventManager.Instance.availableEventIds.Contains(eventId))
        {
            MysteryEventManager.Instance.availableEventIds.Add(eventId);
            Debug.Log($"[UnlockEventEffect] Event ID {eventId} unlocked.");
        }
        else
        {
            Debug.Log($"[UnlockEventEffect] Event ID {eventId} is already unlocked.");
        }
    }
}

