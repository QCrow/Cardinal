using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardScriptable", menuName = "BuildingCardScriptable", order = 0)]
public class BuildingCardScriptable : CardScriptable
{
    [Space(10)]
    [Tooltip("List of traits associated with the building card.")]
    // TODO: This might be changed to be fixed (2 separate variables) instead of a list
    public List<string> Traits = new();
}