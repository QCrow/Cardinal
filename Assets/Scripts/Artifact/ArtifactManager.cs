using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    public static ArtifactManager Instance { get; private set; }

    [SerializeField] private List<ArtifactSO> ownedArtifacts;
    [SerializeField] private ArtifactDatabase artifactDatabase;
    

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void OnTurnStartEffects()
    {
        // Apply all "OnTurnStart" effects for each owned artifact
        foreach (ArtifactSO artifact in ownedArtifacts)
        {
            artifact.ActivateEffect(TriggerCondition.OnTurnStart);
        }
    }

    public void OnDeployActionEffects()
    {
        // Apply all "OnDeploy" effects for each owned artifact
        foreach (ArtifactSO artifact in ownedArtifacts)
        {
            artifact.ActivateEffect(TriggerCondition.OnDeploy);
        }
    }

    public void ObtainArtifactById(int artifactID)
    {
        // e.g., find artifact in your ArtifactDatabase
        ArtifactSO artifact = artifactDatabase.artifacts.Find(a => a.artifactID == artifactID);
        if (artifact != null && !ownedArtifacts.Contains(artifact))
        {
            ownedArtifacts.Add(artifact);
            Debug.Log($"Obtained artifact: {artifact.artifactName}");
            // Trigger its "OnObtain" effects if you want
            artifact.ActivateEffect(TriggerCondition.OnObtain);
        }
        else
        {
            Debug.LogWarning($"Artifact with ID {artifactID} not found or already owned!");
        }
    }

    public void RemoveArtifactById(int artifactID)
    {
        // e.g., find artifact in your ownedArtifacts
        ArtifactSO artifact = ownedArtifacts.Find(a => a.artifactID == artifactID);
        if (artifact != null)
        {
            ownedArtifacts.Remove(artifact);
            Debug.Log($"Removed artifact: {artifact.artifactName}");
        }
        else
        {
            Debug.LogWarning($"Artifact with ID {artifactID} not found in owned list!");
        }
    }
}

