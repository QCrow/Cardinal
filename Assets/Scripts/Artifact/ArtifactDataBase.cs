using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArtifactDatabase", menuName = "Artifacts/Artifact Database")]
public class ArtifactDatabase : ScriptableObject
{
    public List<ArtifactSO> artifacts;
}
