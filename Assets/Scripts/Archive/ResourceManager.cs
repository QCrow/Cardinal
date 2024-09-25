// using System;
// using System.Collections.Generic;
// using UnityEngine;

// /// <summary>
// /// Manages the game's resources such as Energy, Food, and Morale. This class is responsible for
// /// initializing, modifying, and retrieving resource values, as well as updating the UI display.
// /// </summary>
// public class ResourceManager : MonoBehaviour
// {
//     // Singleton instance of the ResourceManager
//     private static ResourceManager _instance;
//     public static ResourceManager Instance => _instance;

//     // Dictionary to store and manage resources by their type
//     private Dictionary<ResourceType, Resource> _resources;

//     // Event triggered whenever a resource's value is changed
//     public event Action<ResourceType, int> OnResourceChanged;

//     private void Awake()
//     {
//         if (_instance == null)
//         {
//             _instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     private void Start()
//     {
//         InitializeResources();
//     }

//     /// <summary>
//     /// Initializes the resource values, populates the resource dictionary, and updates the UI.
//     /// </summary>
//     private void InitializeResources()
//     {
//         _resources = new();

//         foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
//         {
//             _resources.Add(type, new(0, 99));
//         }

//         ResourceUIDisplay resourceUIDisplay = FindObjectOfType<ResourceUIDisplay>();

//         // Initialize the UI if ResourceUIDisplay is found
//         if (resourceUIDisplay != null)
//         {
//             resourceUIDisplay.InitializeUI(
//                 _resources[ResourceType.Energy].CurrentValue,
//                 _resources[ResourceType.Food].CurrentValue,
//                 _resources[ResourceType.Morale].CurrentValue
//             );
//         }
//         else
//         {
//             Debug.LogWarning("ResourceUIDisplay not found in the scene.");
//         }
//     }

//     /// <summary>
//     /// Modifies the current value of the specified resource by a given amount and updates the UI.
//     /// </summary>
//     /// <param name="type">The type of resource to modify (Energy, Food, Morale).</param>
//     /// <param name="amount">The amount to modify the resource by (can be positive or negative).</param>
//     public void ModifyResourceCurrentValueByAmount(ResourceType type, int amount)
//     {
//         if (_resources.TryGetValue(type, out var resource))
//         {
//             resource.CurrentValue += amount;

//             OnResourceChanged?.Invoke(type, resource.CurrentValue);
//         }
//         else
//         {
//             Debug.LogWarning($"Resource type {type} not found in the dictionary.");
//         }
//     }

//     /// <summary>
//     /// Sets the current value of the specified resource to a specific amount, ensuring it's within the valid range.
//     /// </summary>
//     /// <param name="type">The type of resource to set (Energy, Food, Morale).</param>
//     /// <param name="value">The new value to set for the resource.</param>
//     public void SetResourceCurrentValue(ResourceType type, int value)
//     {
//         if (_resources.TryGetValue(type, out var resource))
//         {
//             resource.CurrentValue = Mathf.Clamp(value, 0, resource.MaxValue); // Ensuring the value is within the range
//             OnResourceChanged?.Invoke(type, resource.CurrentValue);
//         }
//         else
//         {
//             Debug.LogWarning($"Resource type {type} not found in the dictionary.");
//         }
//     }

//     /// <summary>
//     /// Retrieves the current value of the specified resource.
//     /// </summary>
//     /// <param name="type">The type of resource to retrieve (Energy, Food, Morale).</param>
//     /// <returns>The current value of the specified resource, or 0 if the resource type is not found.</returns>
//     public int GetResourceCurrentValue(ResourceType type)
//     {
//         return _resources.TryGetValue(type, out var resource) ? resource.CurrentValue : 0;
//     }
// }
