using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CardGen", menuName = "Config", order = 0)]
public class CardGenConfig : SerializedScriptableObject
{
    public List<Dictionary<CardRarityType, int>> CardRarityWeightsPerLevel = new();
}