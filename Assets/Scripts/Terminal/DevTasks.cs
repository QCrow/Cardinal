using UnityEngine;

public class DevTasks
{
    /// <summary>
    /// Sets the resource value for the specified resource type to the given value.
    /// Alias: sr
    /// </summary>
    /// <param name="resourceType">The type of the resource (Energy, Food, Morale).</param>
    /// <param name="value">The value to set for the resource.</param>
    [DevCommand("\nSR\tSet a resouce to certain value\tUsage: sr <type> <amt>", "SR")]
    public static void SetResourceValue(ResourceType resourceType, int value)
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.SetResourceCurrentValue(resourceType, value);
            Debug.Log($"Set {resourceType} to {value}");
        }
        else
        {
            Debug.LogWarning("ResourceManager instance not found.");
        }
    }
}
