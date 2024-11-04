using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StartingDeckScriptable", menuName = "ScriptableObjects/Starting Deck", order = 0)]
public class StartingDeckScriptable : SerializedScriptableObject
{
    public Dictionary<int, int> Deck = new();
}