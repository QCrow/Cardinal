using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardScriptable", menuName = "BuildingCardScriptable", order = 0)]
public class BuildingCardScriptable : CardScriptable
{
    public List<string> Traits = new();
}