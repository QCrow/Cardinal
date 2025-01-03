using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CardRandomGenerationConfig", menuName = "Config", order = 0)]
public class CardRandomGenerationConfig : SerializedScriptableObject
{
    public List<Dictionary<CardRarityType, int>> CardRarityWeightsPerLevel = new();
}