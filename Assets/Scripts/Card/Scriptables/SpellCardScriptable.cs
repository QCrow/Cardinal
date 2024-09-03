using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell Card", menuName = "SpellCardScriptable", order = 0)]
public class SpellCardScriptable : CardScriptable
{
    [Space(10)]
    [InfoBox("Range of the spell's target: \n"
        + "0 = No target (self) \n"
        + "1 = Single slot \n"
        + "3 = 3x3 area centered on the selected slot\n"
        + "etc.\n"
        + "Only odd numbers are valid except 0.")]
    [ValidateInput("IsValidTargetRange", "TargetRange must be 0 or an odd number greater than 0.")]
    public int TargetRange = 0;

    // Custom validation method for the TargetRange property
    private bool IsValidTargetRange(int range)
    {
        return range == 0 || (range > 0 && range % 2 != 0);
    }
}